import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { MemberModel } from 'src/app/models/memberMode';
import { Pagination } from 'src/app/models/pagination';
import { MemberService } from 'src/app/services/member.service';

@Component({

  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  jwtHelper: JwtHelperService = new JwtHelperService;
  pagination:Pagination;
  members:MemberModel[]
  pageNumber=1;
  pageSize=5;
  minAge=18;
  maxAge=99;
  gender:string;
  orderBy:string;


  constructor(
    private memberService:MemberService
  ) { }

  ngOnInit(): void {
    this.refresh();
    this.listMembers();
  }




  refresh() {
    let token = localStorage.getItem("token");
    if (token) {
      let decode = this.jwtHelper.decodeToken(token);
      let gender = Object.keys(decode).filter(x => x.endsWith("/gender"))[0];
      this.gender=decode[gender]=== 'female' ? 'male' : 'female';
      // if (decode[gender]==="male") {
      //   this.gender="female"
      // }
      // if (decode[gender]==="female") {
      //   this.gender="male"
      // }
    }
  }

  listMembers(){
    this.memberService.getMembers(this.pageNumber,this.pageSize,this.minAge,this.maxAge,this.gender,this.orderBy).subscribe((res)=>{
      this.members=res.result;
      this.pagination=res.pagination;
    })
  }

  pageChanged(event:any){
    this.pageNumber=event.page;
    this.listMembers();
  }


  resetFilters(){
    this.minAge=18;
    this.maxAge=99;
    this.listMembers();
  }







}
