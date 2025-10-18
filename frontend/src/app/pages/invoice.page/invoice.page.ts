import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../../services/customer';
import { InvoiceService } from '../../services/invoice';
import { StockService } from '../../services/stock';
import { CustomerDto } from '../../models/dtos.model';
import { ProductDto } from '../../models/dtos.model';
import { InvoiceDto, CreateInvoiceDto } from '../../models/dtos.model';
import { firstValueFrom } from 'rxjs';
import Swal from 'sweetalert2';
import * as signalR from '@microsoft/signalr';

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
  private hubConnection!: signalR.HubConnection;

  customers: CustomerDto[] = [];
  products: ProductDto[] = [];
  invoices: InvoiceDto[] = [];

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

  ngOnInit() {
    this.loadData();

    this.hubConnection = new signalR.HubConnectionBuilder()
          .withUrl('http://localhost:5002/invoiceHub')
          .withAutomaticReconnect()
          .build();

        this.hubConnection.start().catch(err => console.error(err));

        this.hubConnection.on('ReceiveError', (message: string) => {
          const errors: string[] = message.split(',');
          console.log(errors)

          const allErrors = errors.join('<br>');

          Swal.fire({
            icon: 'error',
            title: '⚠️ Notificação',
            html: allErrors,
            showConfirmButton: true
          });

          this.loadData();
        });

        this.hubConnection.on('ReceiveSuccess', (message: string) => {
          console.log(message)
          this.showAlert(message, false, '⚠️ Notificação')
          this.loadData();
        });
  }

  ngOnDestroy() {
    this.hubConnection.stop();
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
        }
      }
    });
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
