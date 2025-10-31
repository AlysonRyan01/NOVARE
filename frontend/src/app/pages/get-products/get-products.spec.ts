import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetProducts } from './get-products';

describe('GetProducts', () => {
  let component: GetProducts;
  let fixture: ComponentFixture<GetProducts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetProducts]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GetProducts);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
