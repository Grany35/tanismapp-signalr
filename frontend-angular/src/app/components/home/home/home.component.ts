import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode=false;

  constructor(
    public authService:AuthService
  ) { }

  ngOnInit(): void {
  }

  registerToggle(){
    this.registerMode=!this.registerMode
  }

  cancelRegisterMode(event:boolean){
    this.registerMode=event;
  }


}
