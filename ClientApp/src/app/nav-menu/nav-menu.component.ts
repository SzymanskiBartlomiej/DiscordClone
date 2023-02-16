import {Component} from '@angular/core';
import {AuthService} from '../services/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  constructor(private auth: AuthService) {
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  isAuthenticated() {
    return this.auth.isAuthenticated();
  }

  getUsername() {
    return this.auth.getUsername();
  }

  async logout() {
    await Promise.all([
      localStorage.setItem('token', ""),
      localStorage.setItem('expiration', ""),
      localStorage.setItem('userID', ""),
      localStorage.setItem('userName', ""),
    ]);
  }
}
