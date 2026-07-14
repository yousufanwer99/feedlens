import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';
import { UploadUrlResponse, VideoResponse, VideoUploadRequest } from '../../shared/models/video.model';

@Injectable({ providedIn: 'root' })
export class VideoService {
  private url = `${environment.apiUrl}/video`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<VideoResponse[]>> {
    return this.http.get<ApiResponse<VideoResponse[]>>(this.url);
  }

  getById(id: number): Observable<ApiResponse<VideoResponse>> {
    return this.http.get<ApiResponse<VideoResponse>>(`${this.url}/${id}`);
  }

  search(query: string): Observable<ApiResponse<VideoResponse[]>> {
    return this.http.get<ApiResponse<VideoResponse[]>>(`${this.url}/search?query=${query}`);
  }

  getMyVideos(): Observable<ApiResponse<VideoResponse[]>> {
    return this.http.get<ApiResponse<VideoResponse[]>>(`${this.url}/my`);
  }

  getUploadUrl(fileName: string, contentType: string): Observable<ApiResponse<UploadUrlResponse>> {
    return this.http.get<ApiResponse<UploadUrlResponse>>(
      `${this.url}/upload-url?fileName=${fileName}&contentType=${contentType}`
    );
  }

  uploadToS3(uploadUrl: string, file: File): Observable<any> {
    return this.http.put(uploadUrl, file, {
      headers: { 'Content-Type': file.type },
      reportProgress: true,
      observe: 'events'
    });
  }

  saveVideo(request: VideoUploadRequest): Observable<ApiResponse<VideoResponse>> {
    return this.http.post<ApiResponse<VideoResponse>>(this.url, request);
  }
}