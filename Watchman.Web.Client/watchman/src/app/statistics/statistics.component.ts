import { Component, OnInit } from '@angular/core';
import { StatisticsService, PeriodStatisticDto } from './services/statistics.service';
import { Subject } from 'rxjs';

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
  xAxisLabel = 'Day';
  showYAxisLabel = true;
  yAxisLabel = 'Messages';

  colorScheme = {
    domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA']
  };

  constructor(private statisticsService: StatisticsService) {
  }

  ngOnInit() {
    this.statisticsService.getStatisticsPerDay().subscribe(x => {
      this.single = [...x.map(s => <any>{ name: this.formatDate(s.timeRange.start), value: s.count })];
      console.log(this.single);
    });
  }

  private formatDate(date: Date): string {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) {
        month = '0' + month;
    }
    if (day.length < 2) {
        day = '0' + day;
    }
    return [year, month, day].join('-');
}

  onSelect(event) {
    console.log(event);
  }

  getData(): any[] {
    return this.single;
  }
}
