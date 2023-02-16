import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {AuthService} from 'src/app/services/auth.service';
import {Message} from 'src/app/interfaces/Message';
import * as signalR from "@microsoft/signalr";
import * as moment from 'moment';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  serverID !: number;
  messages: Message[] = [];
  newMessage !: string;
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
}
