import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {

  constructor(private http: HttpClient) { }

  getStatisticsPerDay(): Observable<PeriodStatisticDto[]> {
    // TODO base URL
    return this.http.get<PeriodStatisticDto[]>('https://localhost:44300/Statistics/GetMessagesStatisticsPerDay');
  }
}

export interface PeriodStatisticDto {
  timeRange: TimeRangeDto;
  count: number;
}

// TODO move to commons
export interface TimeRangeDto {
  start: Date;
  end: Date;
  daysBetween: number;
}
