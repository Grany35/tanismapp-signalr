import { Token } from '@angular/compiler';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister = new EventEmitter();

  registerForm:FormGroup;

  constructor(
    private router:Router,
    private authService:AuthService,
    private toastr:ToastrService,
    private formBuilder:FormBuilder
  ) { }

  ngOnInit(): void {
    this.createRegisterForm();
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

  createRegisterForm(){
    this.registerForm=this.formBuilder.group({
      username:["",Validators.required],
      knownAs:["",Validators.required],
      password:["",Validators.required],
      gender:["",Validators.required],
      city:["",Validators.required],
      country:["",Validators.required],
      dateOfBirth:["",Validators.required]
    })
  }

  register(){
   let token= localStorage.getItem("token");
    if (token) {
      this.toastr.error("zaten bir hesabınız mevcut");
    }
    if (this.registerForm.valid) {
      let registerModel=Object.assign({},this.registerForm.value);
      this.authService.register(registerModel).subscribe((res)=>{
        localStorage.setItem("token",res.data.token);
        this.toastr.success(res.message);
        this.router.navigateByUrl("/member/edit");
      },err=>{
        this.toastr.error(err);
      })
    }
  }

}
