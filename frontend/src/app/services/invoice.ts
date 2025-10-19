import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  InvoiceDto,
  CreateInvoiceDto,
  UpdateInvoiceDto,
  ApiResponse
} from '../models/dtos.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5005/api';

  getInvoices(pageNumber: number = 1, pageSize: number = 20): Observable<ApiResponse<InvoiceDto[]>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ApiResponse<InvoiceDto[]>>(`${this.baseUrl}/invoices`, { params });
  }

  getInvoiceById(id: string): Observable<ApiResponse<InvoiceDto>> {
    return this.http.get<ApiResponse<InvoiceDto>>(`${this.baseUrl}/invoices/${id}`);
  }

  createInvoice(invoice: CreateInvoiceDto): Observable<ApiResponse<InvoiceDto>> {
    return this.http.post<ApiResponse<InvoiceDto>>(`${this.baseUrl}/invoices`, invoice);
  }

  updateInvoice(id: string, invoice: UpdateInvoiceDto): Observable<ApiResponse<InvoiceDto>> {
    return this.http.put<ApiResponse<InvoiceDto>>(`${this.baseUrl}/invoices/${id}`, invoice);
  }

  deleteInvoice(id: string): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.baseUrl}/invoices/${id}`);
  }


  requestPrint(id: string): Observable<ApiResponse<InvoiceDto>> {
    return this.http.post<ApiResponse<InvoiceDto>>(`${this.baseUrl}/invoices/${id}/print`, {});
  }

  getInvoiceStatus(id: string): Observable<ApiResponse<string>> {
    return this.http.get<ApiResponse<string>>(`${this.baseUrl}/invoices/${id}/status`);
  }
}
