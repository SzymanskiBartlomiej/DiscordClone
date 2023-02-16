import {Component, OnInit} from '@angular/core';
import {AuthService} from 'src/app/services/auth.service';
import {Router} from '@angular/router';
import {ServersService} from 'src/app/services/servers.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  login !: string;
  password !: string;
  token !: Object;

  constructor(private auth: AuthService, private router: Router, private serverService: ServersService) {
  }

  ngOnInit(): void {
  }

  async onSubmit() {
    try {
      const token = await this.auth.login(this.login, this.password).toPromise();
      await this.setToken(token);
      this.serverService.updateServers();
      await this.router.navigate(['/']);
    } catch (error) {
      console.error(error);
    }
  }

  async setToken(token: any) {
    const {token: tokenValue, expiration, userID, userName} = token;
    await Promise.all([
      localStorage.setItem('token', tokenValue),
      localStorage.setItem('expiration', expiration),
      localStorage.setItem('userID', userID),
      localStorage.setItem('userName', userName),
    ]);
  }
}
