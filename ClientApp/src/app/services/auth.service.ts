import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private http: HttpClient) {
  }

  login(userName: string, password: string): Observable<any> {
    let params = new HttpParams();
    params = params.append('userName', userName);
    params = params.append('password', password);

    return this.http.post<any>('https://localhost:7034/api/Auth/login', null, {params: params});
  }

  register(userName: string, password: string) {
    let params = new HttpParams();
    params = params.append('userName', userName);
    params = params.append('password', password);
    return this.http.post('https://localhost:7034/api/Auth/register', null, {params: params, observe: "response"});
  }

  UserInfo() {
    return this.http.get('https://localhost:7034/api/Auth/me');
  }

  isAuthenticated() {
    if (localStorage.getItem('token') == null) {
      return false;
    }
    let expiration = localStorage.getItem('expiration');
    if (expiration == null) {
      return false
    }
    const date = moment(expiration, "DD/MM/YYYY h:mm:ssa");
    return date.toDate() > new Date();
  }

  getUsername() {
    return localStorage.getItem('userName');
  }
}
