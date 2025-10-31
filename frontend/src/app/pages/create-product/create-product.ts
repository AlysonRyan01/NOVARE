import { Component } from '@angular/core';
import { CreateProductCard } from '../../components/create-product-card/create-product-card';

@Component({
  selector: 'app-create-product',
  imports: [CreateProductCard],
  templateUrl: './create-product.html',
  styleUrl: './create-product.scss',
})
export class CreateProduct {
  
}
