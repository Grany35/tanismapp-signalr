import { ThisReceiver } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { MemberModel } from 'src/app/models/memberMode';
import { TokenModel } from 'src/app/models/tokenModel';

import { AuthService } from 'src/app/services/auth.service';
import { MemberService } from 'src/app/services/member.service';
import { PresenceService } from 'src/app/services/presence.service';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  jwtHelper: JwtHelperService = new JwtHelperService;


  name: string;
  loginForm: FormGroup
  userName: string = ""
  password: string = ""

  member:MemberModel;


  constructor(
    private formBuilder: FormBuilder,
    public authService: AuthService,
    private router: Router,
    private memberService:MemberService,
    private presenceService:PresenceService
  ) { }

  ngOnInit(): void {
    this.authService.isAuthenticated();

    this.createLoginForm();
    this.refresh()
    this.getUser();
  }

  createLoginForm() {
    this.loginForm = this.formBuilder.group({
      userName: ["", Validators.required],
      password: ["", Validators.required]
    })
  }

  login() {
    if (this.loginForm.valid) {
      let loginModel = Object.assign({}, this.loginForm.value);
      this.authService.login(loginModel).subscribe((res) => {
        console.log(res);
        localStorage.setItem("token", res.data.token);
        const token=localStorage.getItem("token");
        if (token) {
          this.presenceService.createHubConnection();
        }
        this.router.navigateByUrl('/members');
      })
    }
  }

  refresh() {
    let token = localStorage.getItem("token");
    if (token) {
      let decode = this.jwtHelper.decodeToken(token);
      let name = Object.keys(decode).filter(x => x.endsWith("/name"))[0];
      this.name = decode[name];
    }
  }


  getUser(){
    this.memberService.getMember(this.name).subscribe((res)=>{
      this.member=res
    })
  }


  logOut() {
    localStorage.removeItem("token");

    this.presenceService.stopHubConnection();
    this.router.navigateByUrl('/');
  }




}
