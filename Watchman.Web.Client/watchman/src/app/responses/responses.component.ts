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

  updateResponse(f: NgForm) {
    if (!f.valid) {
      return;
    }
    const response = this.responses.filter(x => x.id === f.value.id)[0];
    response.message = f.value.message;
    this.responsesService.updateResponse(response);
  }

  isSaveButtonDisabled(f: NgForm): boolean {
    return this.responses.some(x => x.id === f.value.id && x.message === f.value.message);
  }
}
