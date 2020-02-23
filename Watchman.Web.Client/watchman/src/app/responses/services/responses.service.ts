import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResponsesService {

  constructor(private http: HttpClient) { }

  getResponses(): Observable<ResponseDto[]> {
    return this.http.get<ResponseDto[]>('https://localhost:44300/Responses/GetResponses');
  }

  updateResponse(response: ResponseDto) {

  }
}

export interface ResponseDto {
  id: string;
  onEvent: string;
  message: string;
}

