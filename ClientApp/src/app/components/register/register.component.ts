import {Component, OnInit} from '@angular/core';
import {AuthService} from 'src/app/services/auth.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  login !: string;
  password !: string;
  token !: Object;
  status = false;

  constructor(private auth: AuthService, private router: Router) {
  }

  ngOnInit(): void {
  }

  onSubmit() {
    if(!this.validatePassword()){return}
    if(!this.valiateLogin()){return}
    this.auth.register(this.login, this.password).subscribe(response => {
      if (response.status == 200) {
        this.status = true;
      }
    })
  }
  validatePassword() {
    if(this.password.length < 8) {
      alert('Password must be at least 8 characters long');
      return false;
    }
    if(!/[a-z]/.test(this.password)) {
      alert('Password must contain at least one letter.');
      return false;
    }
    if(!/[0-9]/.test(this.password)){
      alert('Password must contain at least one digit.');
      return false;
    }
    if(!/[A-Z]/.test(this.password)) {
      alert('Password must contain at least one uppercase letter.');
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
