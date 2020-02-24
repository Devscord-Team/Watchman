import { Component, OnInit } from '@angular/core';
import { StatisticsService, PeriodStatisticDto } from './services/statistics.service';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css']
})
export class StatisticsComponent implements OnInit {

  periodStatistics: PeriodStatisticDto[];
  single: any[];

  view: any[] = [700, 400];

  // options
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = true;
  showXAxisLabel = true;
  xAxisLabel = 'Country';
  showYAxisLabel = true;
  yAxisLabel = 'Population';

  colorScheme = {
    domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA']
  };


  constructor(private statisticsService: StatisticsService) {

   }

  ngOnInit() {
    this.statisticsService.getStatisticsPerDay().subscribe(x => {
      this.single = x.map(s => <any>{ name: s.timeRange.start.toDateString(), value: s.count });
    });
  }

  onSelect(event) {
    console.log(event);
  }

  getData(): any[] {
    return this.single;
  }
}
