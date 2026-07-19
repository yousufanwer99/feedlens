export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: number;
  fullName: string;
  email: string;
  token: string;
  role: string;
}

export interface UserProfile {
  id: number;
  fullName: string;
  email: string;
  bio: string | null;
  avatarUrl: string | null;
  preferredCategories: string | null;
  avoidCategories: string | null;
  createdAt: string;
}

export interface UpdateProfileRequest {
  fullName?: string;
  bio?: string;
  preferredCategories?: string;
  avoidCategories?: string;
}
export interface AlgorithmMode {
  name: string;
  tagline: string;
  status: 'available' | 'coming-soon';
  icon: string;
}