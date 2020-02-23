import { Component, OnInit } from '@angular/core';
import { ResponsesService, ResponseDto } from './services/responses.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-responses',
  templateUrl: './responses.component.html',
  styleUrls: ['./responses.component.css']
})
export class ResponsesComponent implements OnInit {

  responses: ResponseDto[];

  constructor(private responsesService: ResponsesService) { }

  ngOnInit() {
    this.responsesService.getResponses().subscribe(x => this.responses = x);
  }

  onSubmit(f: NgForm) {

  }

  isSaveButtonDisabled(f: NgForm): boolean {
    return this.responses.some(x => x.onEvent === f.value.onEvent && x.message === f.value.message);
  }
}
