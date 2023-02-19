import {Component, OnInit} from '@angular/core';
import {AuthService} from 'src/app/services/auth.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  login !: string;
  password !: string;
  token !: Object;

  constructor(private auth: AuthService, private router: Router) {
  }

  ngOnInit(): void {
  }

  async onSubmit() {
    if(!this.validatePassword()){return}
    if(!this.valiateLogin()){return}
    try {
      const token = await this.auth.login(this.login, this.password).toPromise();
      await this.setToken(token);  
      this.auth.authState.next(true);
      await this.router.navigate(['/']);
    } catch (error : any) {
      alert(error.error)
    }
  }

  async setToken(token: any) {
    const {token: tokenValue, expiration, userID, userName , AdminServers} = token;
    await Promise.all([
      localStorage.setItem('token', tokenValue),
      localStorage.setItem('expiration', expiration),
      localStorage.setItem('userID', userID),
      localStorage.setItem('userName', userName),
      localStorage.setItem('AdminServers', AdminServers)
    ]);
  }
  validatePassword() {
    if(this.password.length < 8) {
      alert('Password must be at least 8 characters long');
      return false;
    }
    return true;
  }
  valiateLogin() {
    if(this.password.length < 1){
      alert('Login must be at least 1 character long');
      return false;
    }
    return true;
  }
}
