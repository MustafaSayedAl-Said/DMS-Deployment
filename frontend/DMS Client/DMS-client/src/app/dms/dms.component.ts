import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { IDirectories } from '../shared/Models/Directories';
import { DmsService } from './dms.service';
import { DirectoryParams } from '../shared/Models/DirectoryParams';
import { AccountService } from '../account/account.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dms',
  templateUrl: './dms.component.html',
  styleUrl: './dms.component.scss',
})
export class DmsComponent implements OnInit {
  @ViewChild('search') searchTerm: ElementRef;
  directories: IDirectories[];
  DirectoryParams = new DirectoryParams();
  totalCount: number;
  sortOptions = [
    { name: 'Name Ascending', value: 'NameAsc' },
    { name: 'Name Descending', value: 'NameDesc' },
  ];
  token: string;
  workspaceName: string = 'My Workspace';
  workspaceId: number;
  directoryNameForm: FormGroup;
  showDialog = false;

  constructor(
    private dmsService: DmsService,
    private accountService: AccountService,
    private breadcrumbService: BreadcrumbService,
    private fb: FormBuilder,
    private toast: ToastrService
  ) {}

  ngOnInit(): void {
    this.createDirectoryNameForm();
    this.accountService.getWorkspaceNameFromSubject().subscribe((name) => {
      this.workspaceName = name;
      this.breadcrumbService.set('dms', this.workspaceName);
      console.log('Workspace Name: ', this.workspaceName);
    });
    this.accountService.getWorkspaceIdFromSubject().subscribe((id) => {
      this.workspaceId = id;
      console.log('Workspace id: ' + this.workspaceId);
    });
    this.getDirectories();
  }

  createDirectoryNameForm() {
    this.directoryNameForm = this.fb.group({
      name: ['New Directory', [Validators.required]],
    });
  }
  get _name() {
    return this.directoryNameForm.get('name');
  }
  getDirectories() {
    this.dmsService.getDirectories(this.DirectoryParams).subscribe({
      next: (res) => {
        this.directories = res.data;
        this.totalCount = res.count;
        console.log(this.totalCount);
        this.DirectoryParams.pageNumber = res.pageNumber;
        this.DirectoryParams.pageSize = res.pageSize;
      },
      error: (error) => {
        this.toast.error(error);
      },
      complete: () => {
        console.log('Request completed');
      },
    });
  }

  onSortSelect(sort: Event) {
    let sortValue = (sort.target as HTMLInputElement).value;
    this.DirectoryParams.sort = sortValue;
    this.getDirectories();
  }

  onPageChanged(event: number) {
    this.DirectoryParams.pageNumber = event;
    this.getDirectories();
  }
  OnSearch() {
    this.DirectoryParams.search = this.searchTerm.nativeElement.value;
    this.getDirectories();
  }

  OnReset() {
    this.searchTerm.nativeElement.value = '';
    this.DirectoryParams.search = '';
    this.getDirectories();
  }

  openDialog() {
    this.showDialog = true;
  }

  cancel() {
    this.showDialog = false;
    this.directoryNameForm.setValue({ name: 'New Directory' });
  }

  save() {
    const newName = this._name.value;
    this.dmsService.addDirectory(newName, this.workspaceId).subscribe({
      next: () => {
        this.toast.success('Directory added successfully');
        this.showDialog = false;
        this.directoryNameForm.setValue({ name: 'New Directory' });
        this.getDirectories();
      },
      error: (err) => {
        this.toast.error('Error deleting directory', err);
      },
    });
  }

  onNameUpdate(event: { id: number; newName: string }) {
    const directoryIndex = this.directories.findIndex(
      (dir) => dir.id === event.id
    );
    if (directoryIndex !== -1) {
      this.directories[directoryIndex].name = event.newName;
    }
  }

  onDirectoryDeleted(directoryId: number) {
    console.log('Directory deleted: ', directoryId);

    this.getDirectories();
  }
}
