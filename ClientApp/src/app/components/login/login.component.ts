import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  login !: string;
  password !: string;
  token !: Object;
  constructor(private auth : AuthService , private router : Router) { }

  ngOnInit(): void {
  }
  onSubmit() {
    // TODO Make this function async
    this.auth.login(this.login, this.password).subscribe(token => {
      localStorage.setItem('token', token.token)
      localStorage.setItem('expiration', token.expiration)
      localStorage.setItem('userID', token.userID)
      localStorage.setItem('userName', token.userName)
    });
    this.auth.UserInfo().subscribe(user => console.log(user))
    this.router.navigate(['/']);
  }
}
