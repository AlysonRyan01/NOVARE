import { Component, OnInit, inject } from '@angular/core';
import { CustomerService } from '../../services/customer';
import { CustomerDto, CreateCustomerDto } from '../../models/dtos.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-customers',
  imports: [FormsModule, CommonModule],
  standalone: true,
  templateUrl: './customer.page.html',
  styleUrl: './customer.page.scss'
})
export class CustomersComponent implements OnInit {
  private readonly customerService = inject(CustomerService);

  customers: CustomerDto[] = [];
  loading = false;

  newCustomer: CreateCustomerDto = {
    name: '',
    email: '',
    phone: '',
    document: '',
  };

  ngOnInit(): void {
    this.loadCustomers();
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

  loadCustomers() {
    this.loading = true;
    this.customerService.getCustomers().subscribe({
      next: res => {
        if (res.isSuccess && res.value) {
          this.customers = res.value;
        }
      },
      complete: () => (this.loading = false),
    });
  }

  createCustomer() {
    if (!this.newCustomer.name) return;

    this.customerService.createCustomer(this.newCustomer).subscribe({
      next: res => {
        if (res.isSuccess && res.value) {
          this.customers.push(res.value);
          this.newCustomer = { name: '', email: '', phone: '', document: '' };
          this.showMessage('Cliente criado com sucesso!');
        } else {
          if (res.errors) {
            res.errors.forEach(e => this.showMessage(e, true));
          } else {
            this.showMessage('Erro desconhecido ao criar cliente', true);
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
}
