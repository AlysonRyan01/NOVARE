

// Customer
export interface CustomerDto {
  id: string;
  name: string;
  email: string;
  phone: string;
  document: string;
}

export interface CreateCustomerDto {
  name: string;
  email: string;
  phone: string;
  document: string;
}

// Product
export interface ProductDto {
  id: string;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  createdAt: Date;
  updatedAt: Date;
}

export interface CreateProductDto {
  name: string;
  description: string;
  price: number;
  quantity: number;
}

export interface UpdateProductDto {
  id: string;
  name: string;
  description: string;
  price: number;
}

// Invoice Item
export interface InvoiceItemDto {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
}

// Invoice
export interface InvoiceDto {
  id: string;
  number: string;
  status: string;
  customerId: string;
  total: number;
  createdAt: Date;
  printedAt?: Date | null;
  errors: string[];
  items: InvoiceItemDto[];
}

export interface CreateInvoiceDto {
  customerId: string;
  items: InvoiceItemDto[];
}

export interface UpdateInvoiceDto {
  invoiceId: string;
  customerId: string;
  items: InvoiceItemDto[];
}

export interface ApiResponse<T> {
  isSuccess: boolean;
  value?: T;
  errors?: string[];
  message?: string;
}

// Para respostas de lista
export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface IncreaseStockRequest {
  quantity: number;
}

export interface DecreaseStockRequest {
  quantity: number;
}

export interface IncreaseStockDto {
  productId: string;
  quantity: number;
}

export interface DecreaseStockDto {
  productId: string;
  quantity: number;
}
