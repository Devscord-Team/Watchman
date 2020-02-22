import { Component, OnInit } from '@angular/core';
import { ResponsesService, ResponseDto } from './services/responses.service';
import { Observable } from 'rxjs';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-responses',
  templateUrl: './responses.component.html',
  styleUrls: ['./responses.component.css']
})
export class ResponsesComponent implements OnInit {

  responses: Observable<ResponseDto[]>;

  constructor(private responsesService: ResponsesService) { }

  ngOnInit() {
    this.responses = this.responsesService.getResponses();
  }

  onSubmit(f: NgForm) {
    console.log(f.value);
  }
}
