// services/stock.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ProductDto,
  CreateProductDto,
  UpdateProductDto,
  IncreaseStockRequest,
  DecreaseStockRequest,
  ApiResponse,
  PagedResponse
} from '../models/dtos.model';

@Injectable({
  providedIn: 'root'
})
export class StockService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5005/api';

  getProducts(pageNumber: number = 1, pageSize: number = 20): Observable<ApiResponse<ProductDto[]>> {
  const params = new HttpParams()
    .set('pageNumber', pageNumber.toString())
    .set('pageSize', pageSize.toString());

  return this.http.get<ApiResponse<ProductDto[]>>(`${this.baseUrl}/products`, { params });
}

  getProductById(id: string): Observable<ApiResponse<ProductDto>> {
    return this.http.get<ApiResponse<ProductDto>>(`${this.baseUrl}/products/${id}`);
  }

  createProduct(product: CreateProductDto): Observable<ApiResponse<ProductDto>> {
    return this.http.post<ApiResponse<ProductDto>>(`${this.baseUrl}/products`, product);
  }

  updateProduct(id: string, product: UpdateProductDto): Observable<ApiResponse<ProductDto>> {
    const updateData = { ...product, id };
    return this.http.put<ApiResponse<ProductDto>>(`${this.baseUrl}/products/${id}`, updateData);
  }

  deleteProduct(id: string): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.baseUrl}/products/${id}`);
  }

  increaseStock(id: string, quantity: number): Observable<ApiResponse<ProductDto>> {
    const request: IncreaseStockRequest = { quantity };
    return this.http.patch<ApiResponse<ProductDto>>(
      `${this.baseUrl}/products/${id}/increase-stock`,
      request
    );
  }

  decreaseStock(id: string, quantity: number): Observable<ApiResponse<ProductDto>> {
    const request: DecreaseStockRequest = { quantity };
    return this.http.patch<ApiResponse<ProductDto>>(
      `${this.baseUrl}/products/${id}/decrease-stock`,
      request
    );
  }
}
