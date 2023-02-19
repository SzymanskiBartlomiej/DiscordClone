import {Component, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {AuthService} from '../../services/auth.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {lastValueFrom} from "rxjs";
import {Server} from "../../interfaces/Server";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  name: string = "";
  inviteCode: string = "";
  servers: Server[] = [];

  constructor(private http: HttpClient, private auth: AuthService, private modalService: NgbModal) {
  }

  ngOnInit() {
    this.auth.authState.subscribe(async (authState) => {
      if (this.auth.isAuthenticated()){
        this.getServers();
      }
    })
  }

  async onCreate() {
    let params = new HttpParams();
    params = params.append('name', this.name);
    let url = "https://localhost:7034/api/Servers/create";
    await lastValueFrom(this.http.post<any>(url, null, {params: params}));
    this.name = "";
    this.getServers();
  }

  async onJoin() {
    let params = new HttpParams();
    params = params.append('name', this.name);
    params = params.append('inviteCode', this.inviteCode);
    let url = "https://localhost:7034/api/Servers/join";
    await lastValueFrom(this.http.post<any>(url, null, {params: params}));
    this.inviteCode = "";
    this.name = "";
    this.getServers();
  }

  open(content: any) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'})
  }
  async getServers(){
    let url = "https://localhost:7034/api/Servers ";
    await this.http.get<Server[]>(url).subscribe(servers => {
      this.servers = servers;
    })
  }
}
