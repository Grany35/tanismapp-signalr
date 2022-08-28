import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JwtHelperService } from '@auth0/angular-jwt';
import { ToastrService } from 'ngx-toastr';
import { MemberModel } from 'src/app/models/memberMode';
import { MemberService } from 'src/app/services/member.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  editForm:FormGroup

  jwtHelper:JwtHelperService=new JwtHelperService();
  username:string;
  id:number;
  member:MemberModel

  constructor(
    private memberService:MemberService,
    private formBuilder:FormBuilder,
    private toastr:ToastrService
  ) { }

  ngOnInit(): void {
    this.refresh();
    this.getMember();
    this.createUpdateForm();

  }


  getMember(){
    this.memberService.getMember(this.username).subscribe((res)=>{
      this.member=res
    })
  }

  refresh(){
    let token=localStorage.getItem("token");
    if (token) {
      let decode=this.jwtHelper.decodeToken(token);
      let username= Object.keys(decode).filter(x => x.endsWith("/name"))[0];
      this.username=decode[username];
      let id=Object.keys(decode).filter(x=>x.endsWith("/nameidentifier"))[0];
      this.id=decode[id];
    }
  }

  createUpdateForm(){
    this.editForm = this.formBuilder.group({
      id:[this.id],
      interests:[""],
      introduction:[""],
      lookingFor:[""],
      city:[""],
      country:[""]
    })
  }

  updateMember(){
    if (this.editForm.valid) {
      let updateModel=Object.assign({},this.editForm.value);
      this.memberService.updateMember(updateModel).subscribe((res)=>{
        this.getMember();
        this.toastr.success("Bilgiler kaydedildi")
      })
    }
  }




}
