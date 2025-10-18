import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../../services/customer';
import { InvoiceService } from '../../services/invoice';
import { StockService } from '../../services/stock';
import { CustomerDto } from '../../models/dtos.model';
import { ProductDto } from '../../models/dtos.model';
import { InvoiceDto, CreateInvoiceDto, InvoiceItemDto } from '../../models/dtos.model';
import { interval, Subscription } from 'rxjs';
import { switchMap, startWith } from 'rxjs/operators';
import { firstValueFrom } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'novare-invoice',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './invoice.page.html',
  styleUrl: './invoice.page.scss'
})

export class InvoicePage implements OnInit, OnDestroy {
  private customerService = inject(CustomerService);
  private invoiceService = inject(InvoiceService);
  private stockService = inject(StockService);

  customers: CustomerDto[] = [];
  products: ProductDto[] = [];
  invoices: InvoiceDto[] = [];
  lastInvoiceHash: string = '';

  newInvoice: CreateInvoiceDto = {
    customerId: '',
    items: []
  };

  selectedProductId = '';
  selectedQuantity = 1;

  loading = false;
  formLoading = false;
  error = '';
  formError = '';

  private pollingSubscription?: Subscription;
  private readonly POLLING_INTERVAL = 5000;

  ngOnInit() {
    this.loadData();
    this.startPolling();
  }

  ngOnDestroy() {
    this.stopPolling();
  }

  private showAlert(message: string, isError: boolean = false, title?: string) {
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

  private showConfirmation(title: string, text: string): Promise<any> {
    return Swal.fire({
      title,
      text,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3b82f6',
      cancelButtonColor: '#6b7280',
      confirmButtonText: 'Sim',
      cancelButtonText: 'Cancelar'
    });
  }

  startPolling() {
    this.pollingSubscription = interval(this.POLLING_INTERVAL)
      .pipe(
        startWith(0),
        switchMap(() => this.invoiceService.getInvoices())
      )
      .subscribe({
        next: (response) => {
          if (response.isSuccess && response.value) {
            const newHash = this.generateInvoicesHash(response.value);

            if (newHash !== this.lastInvoiceHash) {
              this.invoices = response.value;
              this.lastInvoiceHash = newHash;
            }
          }
        },
        error: (err) => {
          console.error('Erro no polling de faturas:', err);
          this.showAlert('Erro na sincronização automática', true, '❌ Erro');
        }
      });
  }

  stopPolling() {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
  }

  private generateInvoicesHash(invoices: InvoiceDto[]): string {
    return btoa(JSON.stringify(invoices.map(inv => ({
      id: inv.id,
      status: inv.status,
      total: inv.total,
      items: inv.items.length
    }))));
  }

  loadData() {
    this.loading = true;

    Promise.all([
      firstValueFrom(this.customerService.getCustomers()),
      firstValueFrom(this.stockService.getProducts()),
      firstValueFrom(this.invoiceService.getInvoices())
    ])
    .then(([customersRes, productsRes, invoicesRes]) => {
      this.loading = false;

      if (customersRes?.isSuccess) this.customers = customersRes.value || [];
      if (productsRes?.isSuccess) this.products = productsRes.value || [];
      if (invoicesRes?.isSuccess) {
        this.invoices = invoicesRes.value || [];
        this.lastInvoiceHash = this.generateInvoicesHash(this.invoices);
      }
    })
    .catch(err => {
      this.loading = false;
      this.error = 'Erro ao carregar dados';
      this.showAlert('Erro ao carregar dados', true, '❌ Erro');
    });
  }

  addItemToInvoice() {
    if (!this.selectedProductId || this.selectedQuantity < 1) {
      this.formError = 'Selecione um produto e quantidade válida';
      this.showAlert('Selecione um produto e quantidade válida', true, '⚠️ Aviso');
      return;
    }

    const product = this.products.find(p => p.id === this.selectedProductId);
    if (!product) {
      this.showAlert('Produto não encontrado', true, '❌ Erro');
      return;
    }

    const existingItem = this.newInvoice.items.find(item => item.productId === this.selectedProductId);

    if (existingItem) {
      existingItem.quantity += this.selectedQuantity;
      this.showAlert(`Quantidade de ${product.name} atualizada para ${existingItem.quantity}`, false, '📦 Item atualizado');
    } else {
      this.newInvoice.items.push({
        productId: this.selectedProductId,
        productName: product.name,
        quantity: this.selectedQuantity,
        unitPrice: product.price
      });
    }

    this.selectedProductId = '';
    this.selectedQuantity = 1;
    this.formError = '';
  }

  removeItem(index: number) {
    const removedItem = this.newInvoice.items[index];
    this.newInvoice.items.splice(index, 1);
  }

  getInvoiceTotal(): number {
    return this.newInvoice.items.reduce((total, item) =>
      total + (item.quantity * item.unitPrice), 0);
  }

  getStatusClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'pending',
      'ValidationRequested': 'validation requested',
      'OutOfStock': 'out of stock',
      'Printed': 'printed',
      'Printing': 'printing'
    };
    return statusMap[status] || 'pending';
  }

  getCustomerName(customerId: string): string {
    const customer = this.customers.find(c => c.id === customerId);
    return customer ? customer.name : 'Cliente não encontrado';
  }

  createInvoice() {
    if (!this.newInvoice.customerId || this.newInvoice.items.length === 0) {
      const errorMsg = 'Selecione um cliente e adicione pelo menos um item';
      this.formError = errorMsg;
      this.showAlert(errorMsg, true, '⚠️ Dados incompletos');
      return;
    }

    this.formLoading = true;
    this.formError = '';

    Swal.fire({
      title: 'Criando fatura...',
      text: 'Aguarde enquanto processamos sua solicitação',
      allowOutsideClick: false,
      didOpen: () => {
        Swal.showLoading();
      }
    });

    this.invoiceService.createInvoice(this.newInvoice).subscribe({
      next: (response) => {
        this.formLoading = false;
        Swal.close();

        if (response.isSuccess && response.value) {
          this.resetForm();

          this.forceRefresh();

          this.showAlert(
            `Fatura criada com sucesso!`,
            false,
            '✅ Fatura criada'
          );
        } else {
          const errorMsg = response.errors?.join(', ') || 'Erro ao criar fatura';
          this.showAlert(errorMsg, true, '❌ Erro');
        }
      },
      error: (err) => {
        this.formLoading = false;
        Swal.close();
        this.showAlert('Erro na conexão com o servidor', true, '❌ Erro de conexão');
        console.error('Erro:', err);
      }
    });
  }

  requestPrint(invoice: InvoiceDto) {
    this.showConfirmation(
      'Solicitar impressão',
      `Deseja solicitar a impressão da fatura ${invoice.number}?`
    ).then((result) => {
      if (result.isConfirmed) {

        Swal.fire({
          title: 'Solicitando impressão...',
          text: 'Aguarde enquanto processamos sua solicitação',
          allowOutsideClick: false,
          didOpen: () => {
            Swal.showLoading();
          }
        });

        this.invoiceService.requestPrint(invoice.id).subscribe({
          next: (response) => {
            Swal.close();

            if (response.isSuccess) {

              const invoiceIndex = this.invoices.findIndex(inv => inv.id === invoice.id);
              if (invoiceIndex !== -1) {
                this.invoices[invoiceIndex].status = 'Printing';
              }

              this.forceRefresh();

              this.showAlert(
                `Impressão da fatura ${invoice.number} solicitada com sucesso!`,
                false,
                '🖨️ Impressão'
              );
            } else {
              if (response.errors != undefined)
              response.errors.forEach(erro => {
                this.showAlert(
                erro,
                true,
                '❌ Erro'
              );
              });
            }
          },
          error: (err: any) => {
            Swal.close();
            if (err.error.errors != undefined)
              err.error.errors.forEach((er: any) => {
                this.showAlert(
                er,
                true,
                '❌ Erro'
              );
              });
          }
        });
      }
    });
  }

  private forceRefresh() {
    this.invoiceService.getInvoices().subscribe({
      next: (response) => {
        if (response.isSuccess && response.value) {
          this.invoices = response.value;
          this.lastInvoiceHash = this.generateInvoicesHash(this.invoices);
        }
      }
    });
  }

  manualRefresh() {
    this.showAlert('Atualizando dados...', false, '🔄 Atualizando');
    this.forceRefresh();
  }

  clearForm() {
    if (this.newInvoice.items.length > 0 || this.newInvoice.customerId) {
      this.showConfirmation(
        'Limpar formulário',
        'Tem certeza que deseja limpar o formulário? Todos os itens serão perdidos.'
      ).then((result) => {
        if (result.isConfirmed) {
          this.resetForm();
          this.showAlert('Formulário limpo com sucesso', false, '🧹 Limpo');
        }
      });
    }
  }

  private resetForm() {
    this.newInvoice = {
      customerId: '',
      items: []
    };
    this.selectedProductId = '';
    this.selectedQuantity = 1;
    this.formError = '';
  }
}
