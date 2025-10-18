import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomerDto, CreateCustomerDto, ApiResponse } from '../models/dtos.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api';

  getCustomers(): Observable<ApiResponse<CustomerDto[]>> {
    return this.http.get<ApiResponse<CustomerDto[]>>(`${this.baseUrl}/customers`);
  }

  getCustomerById(id: string): Observable<ApiResponse<CustomerDto>> {
    return this.http.get<ApiResponse<CustomerDto>>(`${this.baseUrl}/customers/${id}`);
  }

  createCustomer(customer: CreateCustomerDto): Observable<ApiResponse<CustomerDto>> {
    return this.http.post<ApiResponse<CustomerDto>>(`${this.baseUrl}/customers`, customer);
  }
}
