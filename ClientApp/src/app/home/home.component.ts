import { Component } from '@angular/core';
import * as signalR from "@microsoft/signalr";

interface Message {
  userName : string;
  message : string;
}
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ["styles.css"],
})
export class HomeComponent {
  private connection : signalR.HubConnection;
  userName !: string;
  newMessage !: string;
  Messages : Message[] = [];
  constructor() {
    this.connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7034/hub").build();
    this.connection.start().then(() => console.log("connection started")).catch(err => console.log("Error during connection " + err));
    this.connection.on("messageReceived",(userName , message) => this.onMessageReceived(userName,message));
  }
  onSendMessage(){
    this.connection.send("NewMessage",this.userName , this.newMessage);
    this.userName = "";
    this.newMessage = "";
  }
  onMessageReceived(userName : string , message : string){
    let tmp =
    {
      userName : userName,
      message : message
    }
    this.Messages.push(tmp)
  }
}
