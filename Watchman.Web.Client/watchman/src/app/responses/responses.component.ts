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
    const response = this.responses.find(x => x.id === f.value.id);
    response.message = f.value.message;
    this.responsesService.updateResponse(response).subscribe();
  }

  isSaveButtonDisabled(f: NgForm): boolean {
    if (!f.valid) {
      return false;
    }
    return this.responses.some(x => x.id === f.value.id && x.message === f.value.message);
  }
}
