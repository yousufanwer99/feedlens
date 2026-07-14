import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginRequest } from '../../../shared/models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  request: LoginRequest = { email: '', password: '' };
  isLoading = false;
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.request).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.isSuccess) {
          this.router.navigate(['/feed']);
        } else {
          this.errorMessage = res.message;
        }
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Something went wrong. Please try again.';
      }
    });
  }
}