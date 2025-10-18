import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicePage } from './invoice.page';

describe('InvoicePage', () => {
  let component: InvoicePage;
  let fixture: ComponentFixture<InvoicePage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoicePage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvoicePage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
