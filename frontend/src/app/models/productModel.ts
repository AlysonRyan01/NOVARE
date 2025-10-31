export interface ProductModel {
  id: string;
  name: string;
  description: string;
  price?: number | null;
  stockQuantity?: number | null;
  createdAt: Date;
  updatedAt: Date;
}
