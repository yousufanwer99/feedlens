import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { VideoService } from '../../../core/services/video.service';
import { LikeService } from '../../../core/services/like.service';
import { AuthService } from '../../../core/services/auth.service';
import { VideoResponse } from '../../../shared/models/video.model';

@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './feed.html',
  styleUrl: './feed.scss'
})
export class Feed implements OnInit {
  videos: VideoResponse[] = [];
  isLoading = true;
  searchQuery = '';
  currentMode = 'Spectrum';

  modes = [
    { name: 'Flare', icon: '🔥', tagline: 'What\'s burning right now', status: 'available' },
    { name: 'Drift', icon: '🎲', tagline: 'Let the feed surprise you', status: 'available' },
    { name: 'Spectrum', icon: '⚡', tagline: 'The full picture, perfectly balanced', status: 'coming-soon' },
    { name: 'Focal', icon: '🎯', tagline: 'Laser focused on your taste', status: 'coming-soon' },
    { name: 'Prism', icon: '🔍', tagline: 'Refracts your interests into new finds', status: 'coming-soon' },
  ];

  constructor(
    private videoService: VideoService,
    private likeService: LikeService,
    public authService: AuthService,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    this.route.queryParams.subscribe(params => {
      if (params['search']) {
        this.searchQuery = params['search'];
        this.searchVideos(params['search']);
      } else {
        this.loadFeed();
      }
    });
  }

  loadFeed(): void {
    this.isLoading = true;
    this.videoService.getFeed(this.currentMode).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.isSuccess) this.videos = res.data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  switchMode(mode: any): void {
    if (mode.status === 'coming-soon') return;
    this.currentMode = mode.name;
    this.loadFeed();

    if (this.authService.isLoggedIn()) {
      this.videoService.updateMode(mode.name).subscribe();
    }
  }

  searchVideos(query: string): void {
    this.isLoading = true;
    this.videoService.search(query).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.isSuccess) this.videos = res.data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleLike(video: VideoResponse, event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    if (!this.authService.isLoggedIn()) return;

    this.likeService.toggleLike(video.id).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          video.isLikedByCurrentUser = res.data;
          video.likeCount += res.data ? 1 : -1;
          this.cdr.detectChanges();
        }
      }
    });
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);
    if (diff < 60) return 'just now';
    if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
    if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
    return `${Math.floor(diff / 86400)}d ago`;
  }

  formatViews(count: number): string {
    if (count >= 1000000) return `${(count / 1000000).toFixed(1)}M`;
    if (count >= 1000) return `${(count / 1000).toFixed(1)}K`;
    return count.toString();
  }
}