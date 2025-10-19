import { Routes } from '@angular/router';
import { CustomersComponent } from './pages/customer.page/customer.page';
import { StockComponent } from './pages/stock.page/stock.page';
import { InvoicePage } from './pages/invoice.page/invoice.page';

export const routes: Routes = [
  { path: 'clientes', component: CustomersComponent },
  { path: 'estoque', component: StockComponent },
  { path: 'notas-fiscais', component: InvoicePage },
  { path: '', redirectTo: 'clientes', pathMatch: 'full' },
];
