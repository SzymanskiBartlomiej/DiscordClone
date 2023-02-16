import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Server} from '../interfaces/Server';
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServersService {
  servers = new BehaviorSubject<Server[]>([]);
  currentServers = this.servers.asObservable();

  constructor(private http: HttpClient) {
  }

  async updateServers() {
    let url = "https://localhost:7034/api/Servers ";
    await this.http.get<Server[]>(url).subscribe(servers => {
      this.servers.next(servers);
    })
  };
}
