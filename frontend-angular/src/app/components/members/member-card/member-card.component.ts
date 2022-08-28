import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MemberModel } from 'src/app/models/memberMode';
import { MemberService } from 'src/app/services/member.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() member:MemberModel;
  

  constructor(
    private memberService:MemberService,
    private toastr:ToastrService,
    public presenceService:PresenceService
    ) { }

  ngOnInit(): void {
  }

  addLike(member:MemberModel){
    this.memberService.addLike(member.userName).subscribe((res)=>{
      this.toastr.success(member.knownAs+'- Kullanıcısını beğendin');
    },err=>{
      this.toastr.error(member.knownAs+'-Kullanıcısını zaten beğendin');
    })
  }

}
