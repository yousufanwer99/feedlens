import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { VideoService } from '../../../core/services/video.service';
import { UserProfile, UpdateProfileRequest } from '../../../shared/models/auth.model';
import { VideoResponse } from '../../../shared/models/video.model';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class Profile implements OnInit {
  profile: UserProfile | null = null;
  myVideos: VideoResponse[] = [];
  isLoading = true;
  isSaving = false;
  isEditing = false;
  activeTab: 'videos' | 'settings' = 'videos';
  successMessage = '';
  errorMessage = '';

  editForm: UpdateProfileRequest = {};

  categories: Category[] = [];

  selectedPreferred: string[] = [];
  selectedAvoid: string[] = [];

  constructor(
    private authService: AuthService,
    private videoService: VideoService,
    private categoryService: CategoryService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadCategories();
    this.loadProfile();
    this.loadMyVideos();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (res) => {
        if (res.isSuccess) this.categories = res.data;
        this.cdr.detectChanges();
      }
    });
  }
  loadProfile(): void {
    this.authService.getProfile().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.isSuccess) {
          this.profile = res.data;
          this.editForm = {
            fullName: res.data.fullName,
            bio: res.data.bio ?? '',
            preferredCategories: res.data.preferredCategories ?? '',
            avoidCategories: res.data.avoidCategories ?? ''
          };
          this.selectedPreferred = this.parseCategories(res.data.preferredCategories);
          this.selectedAvoid = this.parseCategories(res.data.avoidCategories);
        }
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadMyVideos(): void {
    this.videoService.getMyVideos().subscribe({
      next: (res) => {
        if (res.isSuccess) this.myVideos = res.data;
        this.cdr.detectChanges();
      }
    });
  }

  saveProfile(): void {
    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.editForm.preferredCategories = JSON.stringify(this.selectedPreferred);
    this.editForm.avoidCategories = JSON.stringify(this.selectedAvoid);

    this.authService.updateProfile(this.editForm).subscribe({
      next: (res) => {
        this.isSaving = false;
        if (res.isSuccess) {
          this.profile = res.data;
          this.isEditing = false;
          this.successMessage = 'Profile updated successfully';
          setTimeout(() => { this.successMessage = ''; this.cdr.detectChanges(); }, 3000);
        } else {
          this.errorMessage = res.message;
        }
        this.cdr.detectChanges();
      },
      error: () => {
        this.isSaving = false;
        this.errorMessage = 'Failed to update profile';
        this.cdr.detectChanges();
      }
    });
  }

  toggleCategory(category: string, type: 'preferred' | 'avoid'): void {
    const list = type === 'preferred' ? this.selectedPreferred : this.selectedAvoid;
    const index = list.indexOf(category);
    if (index > -1) list.splice(index, 1);
    else list.push(category);
  }

  isSelected(category: string, type: 'preferred' | 'avoid'): boolean {
    return type === 'preferred'
      ? this.selectedPreferred.includes(category)
      : this.selectedAvoid.includes(category);
  }

  parseCategories(json: string | null): string[] {
    if (!json) return [];
    try { return JSON.parse(json); } catch { return []; }
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric', month: 'long', day: 'numeric'
    });
  }

  formatViews(count: number): string {
    if (count >= 1000000) return `${(count / 1000000).toFixed(1)}M`;
    if (count >= 1000) return `${(count / 1000).toFixed(1)}K`;
    return count.toString();
  }
}