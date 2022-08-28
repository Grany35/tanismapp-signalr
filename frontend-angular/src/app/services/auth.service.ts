import { HttpClient } from '@angular/common/http';
import { ThisReceiver } from '@angular/compiler';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginModel } from '../models/loginModel';
import { RegisterModel } from '../models/registerModel';
import { SingleResponseModel } from '../models/singleResponseModel';
import { TokenModel } from '../models/tokenModel';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {


  constructor(
    @Inject('apiUrl') private apiUrl: string,
    private httpClient: HttpClient,
    private presenceService:PresenceService
  ) { }

  login(loginModel: LoginModel) {
    let api = this.apiUrl + 'auth/login';
    return this.httpClient.post<SingleResponseModel<TokenModel>>(api, loginModel);
  }

  isAuthenticated(){
    if(localStorage.getItem("token")){
      return true;
    }
    else{
      return false;
    }
  }

  register(registerModel:RegisterModel){
    let api=this.apiUrl +'auth/register';
    return this.httpClient.post<SingleResponseModel<TokenModel>>(api,registerModel);
  }



}
