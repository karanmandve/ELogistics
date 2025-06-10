import { TestBed } from '@angular/core/testing';

import { SweetAlertToasterService } from './sweet-alert-toaster.service';

describe('SweetAlertToasterService', () => {
  let service: SweetAlertToasterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SweetAlertToasterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
