import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from '@auth0/auth0-angular';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MemberModel } from 'src/app/models/memberMode';
import { Message } from 'src/app/models/message';
import { MemberService } from 'src/app/services/member.service';
import { MessageService } from 'src/app/services/message.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {

  jwtHelper: JwtHelperService = new JwtHelperService;
  @ViewChild('memberTabs',{static:true}) memberTabs: TabsetComponent;

  activeTab: TabDirective
  messages:Message[]=[];
  userName:string;
  member: MemberModel

  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(
    private memberService: MemberService,
    private messageService: MessageService,
    private route: ActivatedRoute,
    public presenceService:PresenceService
  ) { }


  ngOnInit(): void {
    this.getMember();


    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
  }

  getMember() {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(res => {
      console.log(res);
      this.member = res;
      this.galleryImages = this.getImages();
    })
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url
      })
    }
    return imageUrls;
  }

  selectTab(tabId:number){
    this.memberTabs.tabs[tabId].active=true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length===0) {
      this.messageService.createHubConnection(this.member.userName);
    }else{
      this.messageService.stopHubConnection();
    }
  }


  loadMessages(){
    this.messageService.getMessageThread(this.member.userName).subscribe((res)=>{
      this.messages=res;
    })
  }





  ngOnDestroy(): void {
    this.messageService.stopHubConnection();;
  }

}
