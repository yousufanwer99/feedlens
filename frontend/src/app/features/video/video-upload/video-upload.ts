import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { VideoService } from '../../../core/services/video.service';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category.model';

@Component({
  selector: 'app-video-upload',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './video-upload.html',
  styleUrl: './video-upload.scss'
})
export class VideoUpload implements OnInit {
  selectedFile: File | null = null;
  videoPreviewUrl: string | null = null;
  uploadProgress = 0;
  isUploading = false;
  errorMessage = '';
  successMessage = '';
  categories: Category[] = [];

  form = {
    title: '',
    description: '',
    categoryId: 0,
    tags: ''
  };

  constructor(
    private videoService: VideoService,
    private categoryService: CategoryService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.categoryService.getAll().subscribe({
      next: (res) => {
        if (res.isSuccess) this.categories = res.data;
        this.cdr.detectChanges();
      }
    });
  }

  onFileSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    if (!file.type.startsWith('video/')) {
      this.errorMessage = 'Please select a valid video file';
      return;
    }

    if (file.size > 500 * 1024 * 1024) {
      this.errorMessage = 'File size must be less than 500MB';
      return;
    }

    this.selectedFile = file;
    this.errorMessage = '';
    if (this.videoPreviewUrl) URL.revokeObjectURL(this.videoPreviewUrl);
    this.videoPreviewUrl = URL.createObjectURL(file);
    this.cdr.detectChanges();
  }

  onSubmit(): void {
    if (!this.selectedFile) { this.errorMessage = 'Please select a video'; return; }
    if (!this.form.title.trim()) { this.errorMessage = 'Title is required'; return; }
    if (!this.form.categoryId) { this.errorMessage = 'Category is required'; return; }

    this.isUploading = true;
    this.errorMessage = '';
    this.uploadProgress = 0;

    this.videoService.getUploadUrl(this.selectedFile.name, this.selectedFile.type).subscribe({
      next: (res) => {
        if (!res.isSuccess) {
          this.errorMessage = 'Failed to get upload URL';
          this.isUploading = false;
          return;
        }

        const { uploadUrl, s3Key } = res.data;


        this.videoService.uploadToS3(uploadUrl, this.selectedFile!).subscribe({
          next: (percent) => {
            this.uploadProgress = percent;
            this.cdr.detectChanges();
          },
          error: () => {
            this.errorMessage = 'Upload to S3 failed';
            this.isUploading = false;
            this.cdr.detectChanges();
          },
          complete: () => {
            this.uploadProgress = 100;
            this.saveMetadata(s3Key);
            this.cdr.detectChanges();
          }
        });
      },
      error: () => {
        this.errorMessage = 'Something went wrong';
        this.isUploading = false;
        this.cdr.detectChanges();
      }
    });
  }

  private saveMetadata(s3Key: string): void {
    const tags = this.form.tags
      ? JSON.stringify(this.form.tags.split(',').map(t => t.trim()).filter(t => t))
      : null;

    this.videoService.saveVideo({
      title: this.form.title,
      description: this.form.description || null,
      categoryId: this.form.categoryId,
      tags,
      s3Key,
      thumbnailS3Key: null
    }).subscribe({
      next: (res) => {
        this.isUploading = false;
        if (res.isSuccess) {
          this.successMessage = 'Video uploaded successfully!';
          this.cdr.detectChanges();
          setTimeout(() => this.router.navigate(['/video', res.data.id]), 1500);
        } else {
          this.errorMessage = res.message;
          this.cdr.detectChanges();
        }
      },
      error: () => {
        this.isUploading = false;
        this.errorMessage = 'Failed to save video';
        this.cdr.detectChanges();
      }
    });
  }
}