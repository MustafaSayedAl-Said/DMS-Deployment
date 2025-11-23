import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IUsersGet } from '../../../shared/Models/UsersGet';
import { UserParams } from '../../../shared/Models/UserParams';
import { AdminService } from '../admin.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss',
})
export class UsersComponent implements OnInit {
  @ViewChild('search') searchTerm: ElementRef;
  users: IUsersGet[];
  userParams = new UserParams();
  totalCount: number;
  sortField: string = '';
  sortOrder: string = '';

  constructor(
    private adminService: AdminService,
    private toast: ToastrService,
    private router: Router,
    private breadcrumbService: BreadcrumbService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.breadcrumbService.set('users', 'Users');
  }

  loadUsers() {
    this.adminService.getUsersAndWorkspaces(this.userParams).subscribe(
      (res) => {
        this.users = res.data;
        this.totalCount = res.count;
        this.userParams.pageNumber = res.pageNumber;
        this.userParams.pageSize = res.pageSize;
      },
      (error) => {
        this.toast.error(error);
      }
    );
  }

  onSort(field: string) {
    if (this.sortField === field) {
      this.sortOrder = this.sortOrder === 'Asc' ? 'Desc' : 'Asc';
    } else {
      this.sortField = field;
      this.sortOrder = 'Asc';
    }
    this.userParams.sort = this.sortField + this.sortOrder;
    this.loadUsers();
  }

  onPageChanged(event: number) {
    this.userParams.pageNumber = event;
    this.loadUsers();
  }

  OnSearch() {
    this.userParams.search = this.searchTerm.nativeElement.value;
    this.loadUsers();
  }

  OnReset() {
    this.searchTerm.nativeElement.value = '';
    this.userParams.search = '';
    this.sortField = '';
    this.sortOrder = '';
    this.userParams = new UserParams();
    this.loadUsers();
  }

  toggleUserLock(id: number) {
    this.adminService.updateUserLock(id).subscribe({
      next: () => {
        this.toast.success('Lock updated successfully');
        let userIndex = this.users.findIndex((user) => user.id === id);
        if (userIndex !== -1) {
          this.users[userIndex].isLocked = !this.users[userIndex].isLocked;
        }
      },
      error: (err) => {
        this.toast.error('Error updating lock', err);
      },
    });
  }
}
