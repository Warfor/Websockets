import {Component, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule} from "@angular/forms";
import {BaseDto, ServerEchosClientDto} from "../BaseDto";

class ServerBroadcastDto {
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})



export class AppComponent {
  title = 'frontend';

  messages: string[] = [];

  ws: WebSocket;
  messageContent = new FormControl('');
  username = new FormControl("");
  password = new FormControl("");
  roomIdToEnter = new FormControl<number>(0);

  constructor() {
    this.ws = new WebSocket("ws://localhost:8181")
    this.ws.onmessage = message => {
      const messageFromServer = JSON.parse(message.data) as BaseDto<any>;

      console.log(messageFromServer)
      if(message.data) {
        // @ts-ignore
        this.messages.push((message.data))
      }


    }

  }
  ServerEchosClient(dto: ServerEchosClientDto) {
    this.messages.push(dto.echoValue!);
  }

  sendMessage()  {
    var object = {
      eventType: "ClientWantsToEchoServer",
      messageContent: this.messageContent.value!
    }
    this.ws.send(JSON.stringify(object));
  }

  signIn() {
    const signInEvent= {
      eventType: "ClientWantsToSignIn",
      Username: this.username.value
    }
    this.ws.send(JSON.stringify(signInEvent));

  console.log(this.username.value, this.password.value)
  }

  enterRoom() {
    const enterRoomEvent= {
      eventType: "ClientWantsToEnterRoom",
      Username: this.username.value,
      RoomId: this.roomIdToEnter.value
    }
    this.ws.send(JSON.stringify(enterRoomEvent));
    console.log(this.username.value, this.roomIdToEnter);
  }
}
