import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/account-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected creds:any={};
  protected accountService=inject(AccountService);
  protected loggedIn = signal(false);
  login(){
    this.accountService.login(this.creds).subscribe({
      next: (response) => {        console.log(response);
        this.loggedIn.set(true);
        this.creds={};
      },
      error: (error) => {
       this.loggedIn.set(true);
       this.creds={};
      }
    })

  }
  logout(){
     this.loggedIn.set(false);
  }
}
