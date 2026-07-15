import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { VideoService } from '../../../core/services/video.service';
import { LikeService } from '../../../core/services/like.service';
import { AuthService } from '../../../core/services/auth.service';
import { VideoResponse } from '../../../shared/models/video.model';

@Component({
  selector: 'app-video-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './video-detail.html',
  styleUrl: './video-detail.scss'
})
export class VideoDetail implements OnInit {
  video: VideoResponse | null = null;
  isLoading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private videoService: VideoService,
    private likeService: LikeService,
    public authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.videoService.getById(id).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.isSuccess) this.video = res.data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.error = 'Video not found';
        this.cdr.detectChanges();
      }
    });
  }

  toggleLike(): void {
    if (!this.video || !this.authService.isLoggedIn()) return;

    this.likeService.toggleLike(this.video.id).subscribe({
      next: (res) => {
        if (res.isSuccess && this.video) {
          this.video.isLikedByCurrentUser = res.data;
          this.video.likeCount += res.data ? 1 : -1;
          this.cdr.detectChanges();
        }
      }
    });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric', month: 'long', day: 'numeric'
    });
  }

  parseTags(tags: string | null): string[] {
    if (!tags) return [];
    try { return JSON.parse(tags); } catch { return tags.split(','); }
  }
}