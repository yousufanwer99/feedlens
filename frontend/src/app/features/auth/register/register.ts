import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest } from '../../../shared/models/auth.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {
  request: RegisterRequest = { fullName: '', email: '', password: '' };
  confirmPassword = '';
  isLoading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  onSubmit(): void {
    if (!this.request.fullName.trim()) {
      this.errorMessage = 'Full name is required';
      return;
    }
    if (!this.request.email.trim()) {
      this.errorMessage = 'Email is required';
      return;
    }
    if (!this.request.password) {
      this.errorMessage = 'Password is required';
      return;
    }
    if (this.request.password.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters';
      return;
    }
    if (this.request.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.register(this.request).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.isSuccess) {
          this.router.navigate(['/feed']);
        } else {
          this.errorMessage = res.message;
        }
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Something went wrong. Please try again.';
        this.cdr.detectChanges();
      }
    });
  }
}