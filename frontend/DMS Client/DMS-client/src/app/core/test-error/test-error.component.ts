import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { error } from 'console';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrl: './test-error.component.scss'
})
export class TestErrorComponent implements OnInit {

  baseUrl: string = 'https://localhost:7030/api/'
  constructor(private http:HttpClient) {
    
  }
  ngOnInit(): void {
  }

  get404Error(){
    this.http.get(this.baseUrl + 'directory/9484').subscribe({
      next:(next) => console.info(next),
      error: (err) => console.error(err)
    });
  }

  get400ValidationError(){
    this.http.get(this.baseUrl + 'directory/three').subscribe({
      next:(next) => console.info(next),
      error: (err) => console.error(err)
    });
  }

}
