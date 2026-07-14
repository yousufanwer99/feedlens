import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {
  constructor(public authService: AuthService, private router: Router) {}

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
  onSearch(event: Event): void {
  const query = (event.target as HTMLInputElement).value;
  if (query.trim()) {
    this.router.navigate(['/feed'], { queryParams: { search: query } });
  }
}
}