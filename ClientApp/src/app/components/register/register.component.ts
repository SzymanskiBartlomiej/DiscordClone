import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
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
  constructor(private auth : AuthService , private router : Router) { }

  ngOnInit(): void {
  }
  onSubmit() {
    this.auth.register(this.login, this.password).subscribe(response => {
      if (response.status == 200) {
        this.status = true;
      }
    })
  }
}
