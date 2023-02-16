import {Component, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {AuthService} from '../../services/auth.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {lastValueFrom} from "rxjs";
import {Server} from "../../interfaces/Server";
import {ServersService} from 'src/app/services/servers.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  name: string = "";
  inviteCode: string = "";
  servers: Server[] = [];

  constructor(private http: HttpClient, private auth: AuthService, private modalService: NgbModal, private serverService: ServersService) {
  }

  ngOnInit() {
    if (this.auth.isAuthenticated()) {
      this.serverService.updateServers();
      this.serverService.currentServers.subscribe(servers => {
        this.servers = servers;
      });
    }
  }

  async onCreate() {
    let params = new HttpParams();
    params = params.append('name', this.name);

    let url = "https://localhost:7034/api/Servers/create";
    await lastValueFrom(this.http.post<any>(url, null, {params: params}));
    this.name = "";
    this.serverService.updateServers();
  }

  async onJoin() {
    let params = new HttpParams();
    params = params.append('name', this.name);
    params = params.append('inviteCode', this.inviteCode);
    let url = "https://localhost:7034/api/Servers/join";
    await lastValueFrom(this.http.post<any>(url, null, {params: params}));
    this.inviteCode = "";
    this.name = "";
    this.serverService.updateServers();
  }

  open(content: any) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'})
  }
}
