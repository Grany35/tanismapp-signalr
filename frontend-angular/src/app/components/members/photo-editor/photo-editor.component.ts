import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { FileUploader } from 'ng2-file-upload';
import { MemberModel } from 'src/app/models/memberMode';
import { PhotoModel } from 'src/app/models/photoModel';
import { MemberService } from 'src/app/services/member.service';
import { NavComponent } from '../../nav/nav.component';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member: MemberModel;
  jwtHelper: JwtHelperService = new JwtHelperService();

  userName: string;

  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  response: string;

  constructor(
    @Inject('apiUrl') private apiUrl: string,
    private memberService:MemberService
  ) { }

  ngOnInit(): void {
    this.refresh();
    this.imageUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  imageUploader() {
    this.uploader = new FileUploader({
      url: this.apiUrl + 'users/add-photo?userName=' + this.userName,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo=JSON.parse(response);
        this.member.photos.push(photo);
      }
    }
  }

  refresh() {
    let token = localStorage.getItem("token");
    if (token) {
      let decode = this.jwtHelper.decodeToken(token);
      let userName = Object.keys(decode).filter(x => x.endsWith("/name"))[0];
      this.userName = decode[userName];
    }
  }


  setMainPhoto(photo:PhotoModel){
    this.memberService.setMainPhoto(this.userName,photo.id).subscribe((res)=>{
     window.location.reload()
    })
  }

  deletePhoto(photo:PhotoModel){
    this.memberService.deletePhoto(this.userName,photo.id).subscribe((res)=>{
      window.location.reload()
    })
  }


}
