import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, map, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MemberModel } from '../models/memberMode';
import { Message } from '../models/message';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  token = localStorage.getItem("token");
  paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();
  private messageThreadSource=new BehaviorSubject<Message[]>([]);
  messageThread$=this.messageThreadSource.asObservable();

  constructor(
    @Inject('apiUrl') private apiUrl: string,
    private httpClient: HttpClient
  ) { }


  createHubConnection(otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'message?user=' + otherUsername, {
      accessTokenFactory: () => this.token
    })
    .withAutomaticReconnect()
    .build()

    this.hubConnection.start().catch(error=>console.log(error));

    this.hubConnection.on('ReceiveMessageThread',messages=>{
      this.messageThreadSource.next(messages);
    })

    this.hubConnection.on('NewMessage',message=>{
      this.messageThread$.pipe(take(1)).subscribe(messages=>{
        this.messageThreadSource.next([...messages,message]);
      })
    })

  }

  stopHubConnection(){
    if (this.hubConnection) {
      this.hubConnection.stop();
    }

  }


  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = new HttpParams();
    if (pageNumber !== null && pageSize !== null) {
      params = params.append('pageNumber', pageNumber.toString());
      params = params.append('pageSize', pageSize.toString());
    }
    params = params.append('container', container);
    return this.httpClient.get<Message[]>(this.apiUrl + 'messages', { observe: 'response', params }).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return this.paginatedResult;
      })
    );
  }

  getMessageThread(userName: string) {
    return this.httpClient.get<Message[]>(this.apiUrl + 'messages/thread/' + userName);
  }

  async sendMessage(userName: string, content: string) {
    return this.hubConnection.invoke('SendMessage', { recipientUserName: userName, content })
    .catch(error=>console.log(error));
  }

  deleteMessage(id: number) {
    return this.httpClient.get(this.apiUrl + 'messages/' + id);
  }


}
