import { Component, OnInit, inject } from '@angular/core';
import { StockService } from '../../services/stock';
import { ProductDto, CreateProductDto, UpdateProductDto } from '../../models/dtos.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-stock',
  imports: [FormsModule, CommonModule],
  standalone: true,
  templateUrl: './stock.page.html',
  styleUrl: './stock.page.scss'
})
export class StockComponent implements OnInit {
  private readonly stockService = inject(StockService);

  products: ProductDto[] = [];
  loading = false;

  newProduct: CreateProductDto = {
    name: '',
    description: '',
    price: 0,
    quantity: 0
  };

  showStockModal = false;
  stockModalType: 'increase' | 'decrease' = 'increase';
  selectedProductId: string = '';
  stockQuantity: number = 0;

  showEditModal = false;
  editingProduct: UpdateProductDto = {
    id: '',
    name: '',
    description: '',
    price: 0
  };

  ngOnInit(): void {
    this.loadProducts();
  }

  private showMessage(message: string, isError = false) {
    const Toast = Swal.mixin({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      background: isError ? '#fef2f2' : '#f0fdf4',
      color: isError ? '#dc2626' : '#16a34a',
      iconColor: isError ? '#dc2626' : '#16a34a',
      didOpen: (toast) => {
        toast.onmouseenter = Swal.stopTimer;
        toast.onmouseleave = Swal.resumeTimer;
      }
    });

    Toast.fire({
      icon: isError ? 'error' : 'success',
      title: message
    });
  }

  loadProducts() {
    this.loading = true;
    this.stockService.getProducts().subscribe({
      next: res => {
        if (res.isSuccess && res.value) {
          this.products = res.value;
          console.log('Produtos carregados da API:', this.products);
        } else if (res.errors) {
          res.errors.forEach(e => this.showMessage(e, true));
        }
      },
      error: (err) => {
        this.showMessage('Erro ao carregar produtos', true);
        console.error('Erro:', err);
      },
      complete: () => (this.loading = false),
    });
  }

  createProduct() {
    if (!this.newProduct.name) {
      this.showMessage('Nome do produto é obrigatório', true);
      return;
    }

    if (this.newProduct.price < 0) {
      this.showMessage('O preço não pode ser negativo', true);
      return;
    }

    if (this.newProduct.quantity < 0) {
      this.showMessage('A quantidade não pode ser negativa', true);
      return;
    }

    this.stockService.createProduct(this.newProduct).subscribe({
      next: res => {
        if (res.isSuccess && res.value) {
          this.resetNewProductForm();
          this.showMessage('Produto criado com sucesso!');
          this.loadProducts();
        } else {
          if (res.errors) {
            res.errors.forEach(e => this.showMessage(e, true));
          } else {
            this.showMessage('Erro desconhecido ao criar produto', true);
          }
        }
      },
      error: err => {
        console.error('Erro HTTP:', err);
        if (err.error && err.error.errors) {
          err.error.errors.forEach((e: string) => this.showMessage(e, true));
        } else if (err.error && err.error.message) {
          this.showMessage(err.error.message, true);
        } else {
          this.showMessage('Erro inesperado na requisição', true);
        }
      }
    });
  }

  private resetNewProductForm() {
    this.newProduct = {
      name: '',
      description: '',
      price: 0,
      quantity: 0
    };
  }

  editProduct(product: ProductDto) {
    this.editingProduct = {
      id: product.id,
      name: product.name,
      description: product.description,
      price: product.price
    };
    this.showEditModal = true;
  }

  updateProduct() {
    if (!this.editingProduct.name) {
      this.showMessage('Nome do produto é obrigatório', true);
      return;
    }

    if (this.editingProduct.price < 0) {
      this.showMessage('O preço não pode ser negativo', true);
      return;
    }

    this.stockService.updateProduct(this.editingProduct.id, this.editingProduct).subscribe({
      next: res => {
        if (res.isSuccess && res.value) {
          this.closeEditModal();
          this.showMessage('Produto atualizado com sucesso!');
          this.loadProducts();
        } else {
          if (res.errors) {
            res.errors.forEach(e => this.showMessage(e, true));
          } else {
            this.showMessage('Erro desconhecido ao atualizar produto', true);
          }
        }
      },
      error: err => {
        console.error('Erro HTTP:', err);
        if (err.error && err.error.errors) {
          err.error.errors.forEach((e: string) => this.showMessage(e, true));
        } else if (err.error && err.error.message) {
          this.showMessage(err.error.message, true);
        } else {
          this.showMessage('Erro inesperado na requisição', true);
        }
      }
    });
  }

  closeEditModal() {
    this.showEditModal = false;
    this.editingProduct = {
      id: '',
      name: '',
      description: '',
      price: 0
    };
  }

  deleteProduct(productId: string) {
    Swal.fire({
      title: 'Tem certeza?',
      text: "Esta ação não pode ser revertida!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#6b7280',
      confirmButtonText: 'Sim, excluir!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.stockService.deleteProduct(productId).subscribe({
          next: res => {
            if (res.isSuccess) {
              this.showMessage('Produto excluído com sucesso!');
              this.products = this.products.filter(p => p.id !== productId);
              setTimeout(() => {
                this.loadProducts();
              }, 300);
            } else {
              if (res.errors) {
                res.errors.forEach(e => this.showMessage(e, true));
              } else {
                this.showMessage('Erro ao excluir produto', true);
              }
            }
          },
          error: err => {
            this.showMessage('Erro ao excluir produto', true);
            console.error('Erro:', err);
          }
        });
      }
    });
    }

  showIncreaseStockModal() {
    this.stockModalType = 'increase';
    this.showStockModal = true;
    this.selectedProductId = '';
    this.stockQuantity = 0;
  }

  showDecreaseStockModal() {
    this.stockModalType = 'decrease';
    this.showStockModal = true;
    this.selectedProductId = '';
    this.stockQuantity = 0;
  }

  closeStockModal() {
    this.showStockModal = false;
    this.selectedProductId = '';
    this.stockQuantity = 0;
  }

  processStockOperation() {
    if (!this.selectedProductId || !this.stockQuantity || this.stockQuantity <= 0) {
      this.showMessage('Selecione um produto e informe uma quantidade válida', true);
      return;
    }

    const product = this.products.find(p => p.id === this.selectedProductId);
    if (!product) {
      this.showMessage('Produto não encontrado', true);
      return;
    }

    if (this.stockModalType === 'increase') {
      this.stockService.increaseStock(this.selectedProductId, this.stockQuantity).subscribe({
        next: res => {
          if (res.isSuccess && res.value) {
            this.showMessage(`Estoque aumentado em ${this.stockQuantity} unidades!`);
            this.closeStockModal();
            this.loadProducts();
          } else {
            if (res.errors) {
              res.errors.forEach(e => this.showMessage(e, true));
            } else {
              this.showMessage('Erro ao aumentar estoque', true);
            }
          }
        },
        error: err => {
          this.showMessage('Erro ao aumentar estoque', true);
          console.error('Erro:', err);
        }
      });
    } else {
      if (this.stockQuantity > product.stockQuantity) {
        this.showMessage('Quantidade indisponível em estoque', true);
        return;
      }

      this.stockService.decreaseStock(this.selectedProductId, this.stockQuantity).subscribe({
        next: res => {
          if (res.isSuccess && res.value) {
            this.showMessage(`Estoque reduzido em ${this.stockQuantity} unidades!`);
            this.closeStockModal();
            this.loadProducts();
          } else {
            if (res.errors) {
              res.errors.forEach(e => this.showMessage(e, true));
            } else {
              this.showMessage('Erro ao reduzir estoque', true);
            }
          }
        },
        error: err => {
          this.showMessage('Erro ao reduzir estoque', true);
          console.error('Erro:', err);
        }
      });
    }
  }

  getStockStatus(product: ProductDto): string {
    if (product.stockQuantity === 0) {
      return 'Sem Estoque';
    } else if (product.stockQuantity <= 10) {
      return 'Estoque Baixo';
    } else {
      return 'Em Estoque';
    }
  }

  isLowStock(product: ProductDto): boolean {
    return product.stockQuantity <= 10;
  }

  isOutOfStock(product: ProductDto): boolean {
    return product.stockQuantity === 0;
  }
}
