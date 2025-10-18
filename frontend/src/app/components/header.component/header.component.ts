import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'novare-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="navbar">
      <div class="nav-container">
        <div class="nav-logo">
          <a routerLink="/">Sistema</a>
        </div>
        <ul class="nav-menu">
          <li class="nav-item">
            <a
              routerLink="/clientes"
              routerLinkActive="active"
              class="nav-link">
              Clientes
            </a>
          </li>
          <li class="nav-item">
            <a
              routerLink="/estoque"
              routerLinkActive="active"
              class="nav-link">
              Estoque
            </a>
          </li>
          <li class="nav-item">
            <a
              routerLink="/notas-fiscais"
              routerLinkActive="active"
              class="nav-link">
              Notas Fiscais
            </a>
          </li>
        </ul>
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      background-color: #2c3e50;
      padding: 0 2rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .nav-container {
      max-width: 1200px;
      margin: 0 auto;
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: 60px;
    }

    .nav-logo a {
      color: white;
      text-decoration: none;
      font-size: 1.5rem;
      font-weight: bold;
    }

    .nav-menu {
      display: flex;
      list-style: none;
      margin: 0;
      padding: 0;
      gap: 2rem;
    }

    .nav-link {
      color: #ecf0f1;
      text-decoration: none;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      transition: all 0.3s ease;
      font-weight: 500;
    }

    .nav-link:hover {
      background-color: #34495e;
      color: white;
    }

    .nav-link.active {
      background-color: #3498db;
      color: white;
    }
  `]
})
export class HeaderComponent {}
