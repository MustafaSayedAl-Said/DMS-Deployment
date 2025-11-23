import { Component, Input, OnInit } from '@angular/core';
import { AccountService } from '../../account/account.service';
import { Observable } from 'rxjs';
import { IUser } from '../../shared/Models/user';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss',
})
export class NavBarComponent implements OnInit {
  isAdmin = false;
  constructor(private accountService: AccountService) {}
  currentUser$: Observable<IUser>;
  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
    this.accountService.currentUser$.subscribe((user) => {
      this.isAdmin = user?.isAdmin;
    });
  }

  logout() {
    this.accountService.logout();
  }
}
