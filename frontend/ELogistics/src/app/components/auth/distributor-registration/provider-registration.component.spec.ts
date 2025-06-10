import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DistributorRegistrationComponent } from './distributor-registration.component';

describe('DistributorRegistrationComponent', () => {
  let component: DistributorRegistrationComponent;
  let fixture: ComponentFixture<DistributorRegistrationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DistributorRegistrationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DistributorRegistrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
