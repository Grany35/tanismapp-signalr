import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NavComponent } from './components/nav/nav.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MemberListComponent } from './components/members/member-list/member-list.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MemberCardComponent } from './components/members/member-card/member-card.component';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { MemberDetailComponent } from './components/members/member-detail/member-detail.component';
import { JwtHelperService, JwtInterceptor, JwtModule } from '@auth0/angular-jwt';
import { AuthhInterceptor } from './interceptors/authh.interceptor';
import { Auth0ClientFactory, Auth0ClientService, AuthClientConfig, AuthConfigService } from '@auth0/auth0-angular';
import { config } from 'rxjs';
import { ToastrModule } from 'ngx-toastr';
import { HomeComponent } from './components/home/home/home.component';
import { RegisterComponent } from './components/register/register.component';
import { MemberEditComponent } from './components/members/member-edit/member-edit.component';
import { PhotoEditorComponent } from './components/members/photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from 'ngx-timeago';
import { ListsComponent } from './components/lists/lists.component';
import { MessagesComponent } from './components/messages/messages.component';
import { MemberMessagesComponent } from './components/members/member-messages/member-messages.component';
@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    MemberListComponent,
    MemberCardComponent,
    MemberDetailComponent,
    HomeComponent,
    RegisterComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    ListsComponent,
    MessagesComponent,
    MemberMessagesComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    TooltipModule,
    FormsModule,
    HttpClientModule,
    TimeagoModule.forRoot(),
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    ReactiveFormsModule,
    ToastrModule.forRoot({
      timeOut:3000,
      positionClass:'toast-bottom-right'
    }),
    BrowserAnimationsModule,
    TabsModule.forRoot(),
    NgxGalleryModule,
    FileUploadModule

  ],
  providers: [
    {provide: 'apiUrl',useValue:'https://localhost:7016/api/'},
    {provide:HTTP_INTERCEPTORS,useClass:AuthhInterceptor,multi:true},
    {provide:AuthConfigService,useValue:config},
    {provide:Auth0ClientService,useFactory:Auth0ClientFactory.createClient,deps:[AuthClientConfig]},
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
