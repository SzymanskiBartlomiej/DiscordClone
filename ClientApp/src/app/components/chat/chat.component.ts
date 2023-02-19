import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient, HttpParams} from '@angular/common/http';
import {AuthService} from 'src/app/services/auth.service';
import {Message} from 'src/app/interfaces/Message';
import * as signalR from "@microsoft/signalr";
import * as moment from 'moment';
import { NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';
@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  serverID !: number;
  messages: Message[] = [];
  newMessage !: string;
  isAdmin = false;
  users: string[] = [];
  isCollapsed = true;
  inviteCode = "";
  private connection: signalR.HubConnection;

  constructor(private auth: AuthService, private router: Router, private http: HttpClient, private route: ActivatedRoute) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7034/hub", {accessTokenFactory: () => localStorage.getItem('token') || ""})
      .build();
    let id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.serverID = parseInt(id);
    } else {
      this.router.navigate(['/']);
    }
    if (localStorage.getItem("AdminServers")?.includes(this.serverID.toString())) {
      this.isAdmin = true;
    }
  }

  ngOnInit(): void {
    if (!this.auth.isAuthenticated()) {
      this.router.navigate(['/login']);
    }
    let url = "https://localhost:7034/api/Messages/" + this.serverID.toString()
    this.http.get<Message[]>(url).subscribe(messages => {
      this.messages = messages
    });
    this.connection.start().then(() => {
      console.log("connected")
    }).catch(err => console.log(err));
    this.connection.on("messageReceived", (message: string) => this.OnMessageReceived(message));
    this.http.get<string[]>("https://localhost:7034/api/Servers/users?serverId=" + this.serverID.toString())
    .subscribe(users => {this.users = users;})
    if(this.isAdmin){
      this.http.get<string>("https://localhost:7034/api/Admin/inviteCode?serverId=" + this.serverID.toString())
      .subscribe(inviteCode => this.inviteCode = inviteCode)
    }
  }

  onSend() {
    this.connection.invoke("NewMessage", this.serverID, this.newMessage)
    this.newMessage = ""
  }

  OnMessageReceived(json: string) {
    let message: Message = JSON.parse(json);
    if (message.ServerId == this.serverID) {
      this.messages.push(message);
    }
  }

  fixDate(date: Date) {
    return moment(date).format("DD/MM/YYYY h:mm:ssa");
  }
  onDeleteMessage(messageId : number){
    let params = new HttpParams();
    params = params.append('serverId', this.serverID);
    params = params.append('mesageId', messageId);
    let options = {params: params};
    let url = "https://localhost:7034/api/Admin/DeleteMessage"
    return this.http.delete<any>(url, options).subscribe(
      (response) => {
        this.messages = this.messages.filter((m : Message) => m.MessageId != messageId)
      },
      (error) => {
        console.log(error);
      })
  }
  onDeleteUser(user : string){
    let params = new HttpParams();
    params = params.append('serverId', this.serverID);
    params = params.append('userName', user);
    let options = {params: params};
    let url = "https://localhost:7034/api/Admin/DeleteUser"
    return this.http.delete<any>(url, options).subscribe(
      (response) => {
        this.users = this.users.filter((u : string) => u != user)
      },
      (error) => {
        console.log(error);
      })
  }
  addAdmin(user:string){
    let params = new HttpParams();
    params = params.append('serverId', this.serverID);
    params = params.append('userName', user);
    let options = {params: params};
    let url = "https://localhost:7034/api/Admin/addAdmin"
    return this.http.patch(url,"",options).subscribe(
      (response) => {
        console.log(response)
      },
      (error) => {
        alert(error.error)
      })
  }
}
