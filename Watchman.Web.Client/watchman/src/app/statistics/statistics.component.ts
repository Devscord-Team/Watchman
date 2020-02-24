import { Component, OnInit } from '@angular/core';
import { StatisticsService, PeriodStatisticDto } from './services/statistics.service';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css']
})
export class StatisticsComponent implements OnInit {

  periodStatistics: PeriodStatisticDto[];

  constructor(private statisticsService: StatisticsService) { }

  ngOnInit() {
    this.statisticsService.getStatisticsPerDay().subscribe(x => this.periodStatistics = x)
  }

}
