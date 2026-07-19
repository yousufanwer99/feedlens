import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';
import { UploadUrlResponse, VideoResponse, VideoUploadRequest } from '../../shared/models/video.model';

@Injectable({ providedIn: 'root' })
export class VideoService {
  private url = `${environment.apiUrl}/video`;

  constructor(private http: HttpClient) { }

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

  uploadToS3(uploadUrl: string, file: File): Observable<number> {
    return new Observable(observer => {
      const xhr = new XMLHttpRequest();
      xhr.open('PUT', uploadUrl, true);
      xhr.setRequestHeader('Content-Type', file.type);

      xhr.upload.onprogress = (event) => {
        if (event.lengthComputable) {
          const percent = Math.round((event.loaded / event.total) * 100);
          observer.next(percent);
        }
      };

      xhr.onload = () => {
        if (xhr.status === 200) {
          observer.complete();
        } else {
          observer.error(xhr.responseText);
        }
      };

      xhr.onerror = () => observer.error('Upload failed');
      xhr.send(file);
    });
  }

  saveVideo(request: VideoUploadRequest): Observable<ApiResponse<VideoResponse>> {
    return this.http.post<ApiResponse<VideoResponse>>(this.url, request);
  }
  deleteVideo(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.url}/${id}`);
  }
  recordWatch(videoId: number, watchedSeconds: number, totalSeconds: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.url}/${videoId}/watch`, {
      watchedSeconds,
      totalSeconds
    });
  }
  updateMode(mode: string): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.url}/mode/${mode}`, {});
  }
  getFeed(mode: string): Observable<ApiResponse<VideoResponse[]>> {
    return this.http.get<ApiResponse<VideoResponse[]>>(`${this.url}/feed/${mode}`);
  }
}