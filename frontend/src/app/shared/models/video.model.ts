export interface VideoUploadRequest {
  title: string;
  description: string | null;
  category: string;
  tags: string | null;
  s3Key: string;
  thumbnailS3Key: string | null;
}

export interface VideoResponse {
  id: number;
  title: string;
  description: string | null;
  category: string;
  tags: string | null;
  videoUrl: string;
  thumbnailUrl: string | null;
  viewCount: number;
  likeCount: number;
  isLikedByCurrentUser: boolean;
  userId: number;
  uploaderName: string;
  createdAt: string;
}

export interface UploadUrlResponse {
  uploadUrl: string;
  s3Key: string;
}