import { Routes } from '@angular/router';
import { CreateProduct } from './pages/create-product/create-product';
import { GetProducts } from './pages/get-products/get-products';

export const routes: Routes = [
  {
    path: "criar-produto",
    component: CreateProduct
  },
  {
    path: "obter-produtos",
    component: GetProducts
  }
];
