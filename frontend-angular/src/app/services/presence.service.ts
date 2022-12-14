import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MemberModel } from '../models/memberMode';
import { TokenModel } from '../models/tokenModel';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  token = localStorage.getItem("token");
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(
    private toastr: ToastrService
  ) { }


  createHubConnection() {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'presence', {
      accessTokenFactory: () => this.token
    })
      .withAutomaticReconnect().build()

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.toastr.info(username + ' has connected');
    })

    this.hubConnection.on('UserIsOffline', username => {
      this.toastr.warning(username + ' has disconnected');
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })

  }


  stopHubConnection() {
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
