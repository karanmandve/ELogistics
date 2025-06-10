import { TestBed } from '@angular/core/testing';
import { CountryStateServiceService } from './country-state-service.service';



describe('CountryStateServiceService', () => {
  let service: CountryStateServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CountryStateServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
