import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/productService';
import {MatCardModule} from '@angular/material/card'
import { MatButtonModule } from '@angular/material/button';
import { ProductModel } from '../../models/productModel';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-create-product-card',
  imports: [
    MatCardModule,
    MatButtonModule
  ],
  templateUrl: './create-product-card.html',
  styleUrl: './create-product-card.scss',
})
export class CreateProductCard {
  private _productService = inject(ProductService);
  private _messageService = inject(MessageService);
  isLoading: boolean = false;

  productInput: ProductModel = {
    id: '',
    name: '',
    description: '',
    price: 0,
    stockQuantity: 0,
    createdAt: new Date(),
    updatedAt: new Date()
  }

  createProduct(): void {
    if (this.isLoading) return;

    const errors = this.validateInput(this.productInput);
    if (errors.length > 0) {
      this.errorSnackBar(errors[0])
      return;
    }

    this.isLoading = true;

    this._productService.createProduct(this.productInput).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (!response.IsSuccess) {
          this.errorSnackBar(response.Errors[0]);
          return;
        }

        this.successSnackBar("Produto criado com sucesso");
        this.resetForm();
      },
      error: () => {
        this.isLoading = false;
        this.errorSnackBar("Ocorreu um erro ao criar o produto");
      }
    })
  }

  successSnackBar(message:string) {
    this._messageService.add({ severity: 'success', summary: 'Sucesso!', detail: message, life: 3000 });
  }

  errorSnackBar(message:string) {
    this._messageService.add({ severity: 'error', summary: 'Erro!', detail: message, life: 3000 });
  }

  validateInput(inputModel: ProductModel): string[] {
    const errors: string[] = [];

    if (!inputModel.name || inputModel.name.trim().length === 0) {
      errors.push("O nome do produto é obrigatório");
    } else {
      const trimmedName = inputModel.name.trim();

      if (trimmedName.length < 2) {
        errors.push("O nome deve ter pelo menos 2 caracteres");
      }

      if (trimmedName.length > 200) {
        errors.push("O nome não pode exceder 200 caracteres");
      }

      const nameRegex = /^[\p{L}\p{N}\s.,'’"-]+$/u;
      if (!nameRegex.test(trimmedName)) {
        errors.push("O nome contém caracteres inválidos");
      }
    }

    if (!inputModel.description || inputModel.description.trim().length === 0) {
      errors.push("A descrição do produto é obrigatória");
    } else {
      const trimmedDescription = inputModel.description.trim();

      if (trimmedDescription.length < 10) {
        errors.push("A descrição deve ter pelo menos 10 caracteres");
      }

      if (trimmedDescription.length > 1000) {
        errors.push("A descrição não pode exceder 1000 caracteres");
      }

      const descriptionRegex = /^[\p{L}\p{N}\s.,'’"()-]+$/u;
      if (!descriptionRegex.test(trimmedDescription)) {
        errors.push("A descrição contém caracteres inválidos");
      }
    }

    if (inputModel.price <= 0) {
      errors.push("O preço deve ser maior que zero");
    } else {
      if (inputModel.price > 999999.99) {
        errors.push("O preço não pode exceder 999.999,99");
      }

      if (!/^\d+(\.\d{1,2})?$/.test(inputModel.price.toString())) {
        errors.push("O preço deve ter no máximo 2 casas decimais");
      }
    }

    if (inputModel.stockQuantity < 0) {
      errors.push("A quantidade em estoque não pode ser negativa");
    } else if (inputModel.stockQuantity > 100000) {
      errors.push("A quantidade não pode exceder 100.000 unidades");
    }

    return errors;
  }

  resetForm(): void {
    this.productInput = {
      id: '',
      name: '',
      description: '',
      price: 0,
      stockQuantity: 0,
      createdAt: new Date(),
      updatedAt: new Date()
    };
  }
}
