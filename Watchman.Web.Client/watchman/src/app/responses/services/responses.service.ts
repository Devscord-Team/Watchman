import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResponsesService {

  constructor(private http: HttpClient) { }

  getResponses(): Observable<ResponseDto[]> {
    // TODO base URL
    return this.http.get<ResponseDto[]>('https://localhost:44300/Responses/GetResponses');
  }

  updateResponse(request: ResponseDto) {
    // TODO base URL
    return this.http.post('https://localhost:44300/Responses/UpdateResponse', request);
  }
}

export interface ResponseDto {
  id: string;
  onEvent: string;
  message: string;
}

