import { Injectable, inject } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductModel } from "../models/productModel";
import { ApiResponse } from "../models/apiResponse";

@Injectable({
  providedIn: 'root'
})

export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5005/api';

  createProduct(model: ProductModel): Observable<ApiResponse<ProductModel>> {
    return this.http.post<ApiResponse<ProductModel>>(`${this.baseUrl}/products`, model);
  }

  getAllProducts(pageNumber: number = 1, pageSize: number = 20): Observable<ApiResponse<ProductModel[]>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ApiResponse<ProductModel[]>>(`${this.baseUrl}/products`, { params });
  }

  getProductById(id: string): Observable<ApiResponse<ProductModel>> {
    return this.http.get<ApiResponse<ProductModel>>(`${this.baseUrl}/products/${id}`)
  }

  updateProduct(model: ProductModel): Observable<ApiResponse<ProductModel>> {
    return this.http.put<ApiResponse<ProductModel>>(`${this.baseUrl}/products/${model.id}`, model);
  }

  deleteProduct(id: string): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.baseUrl}/products/${id}`);
  }
}
