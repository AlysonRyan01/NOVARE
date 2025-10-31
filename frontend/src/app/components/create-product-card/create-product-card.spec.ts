import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateProductCard } from './create-product-card';

describe('CreateProductCard', () => {
  let component: CreateProductCard;
  let fixture: ComponentFixture<CreateProductCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateProductCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateProductCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
