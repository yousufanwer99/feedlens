import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'feed', pathMatch: 'full' },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login').then(m => m.Login)
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register').then(m => m.Register)
      }
    ]
  },
  {
    path: 'feed',
    loadComponent: () => import('./features/feed/feed/feed').then(m => m.Feed)
  },
  {
    path: 'video/:id',
    loadComponent: () => import('./features/video/video-detail/video-detail').then(m => m.VideoDetail)
  },
  {
    path: 'upload',
    canActivate: [authGuard],
    loadComponent: () => import('./features/video/video-upload/video-upload').then(m => m.VideoUpload)
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () => import('./features/profile/profile/profile').then(m => m.Profile)
  },
  { path: '**', redirectTo: 'feed' }
];