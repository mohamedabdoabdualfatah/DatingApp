import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { single } from 'rxjs';
import { Nav } from '../layout/nav/nav';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  imports:[Nav],
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly http:HttpClient=inject(HttpClient);
  protected readonly title:string = 'client'
  protected members=signal<any>([]);
  ngOnInit(): void {
   this.http.get("https://localhost:7066/api/members").subscribe({
    next: response => {
      this.members.set(response);
      console.log(response);
    },
    error: error => console.log(error),
    complete: () => console.log("Request completed")
   })
  }
}
