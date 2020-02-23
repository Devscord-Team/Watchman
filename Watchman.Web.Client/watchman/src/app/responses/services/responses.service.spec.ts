/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ResponsesService } from './responses.service';

describe('Service: Responses', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ResponsesService]
    });
  });

  it('should ...', inject([ResponsesService], (service: ResponsesService) => {
    expect(service).toBeTruthy();
  }));
});
