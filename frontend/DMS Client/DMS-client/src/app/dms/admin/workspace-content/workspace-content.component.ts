import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IDirectories } from '../../../shared/Models/Directories';
import { DirectoryParams } from '../../../shared/Models/DirectoryParams';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AdminService } from '../admin.service';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ToastrService } from 'ngx-toastr';
import { DmsService } from '../../dms.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-workspace-content',
  templateUrl: './workspace-content.component.html',
  styleUrl: './workspace-content.component.scss',
})
export class WorkspaceContentComponent implements OnInit {
  @ViewChild('search') searchTerm: ElementRef;
  directories: IDirectories[];
  DirectoryParams = new DirectoryParams();
  totalCount: number;
  sortOptions = [
    { name: 'Name Ascending', value: 'NameAsc' },
    { name: 'Name Descending', value: 'NameDesc' },
  ];
  workspaceName: string;
  workspaceId: number;
  directoryNameForm: FormGroup;
  showDialog = false;

  constructor(
    private adminService: AdminService,
    private breadcrumbService: BreadcrumbService,
    private fb: FormBuilder,
    private toast: ToastrService,
    private dmsService: DmsService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.createDirectoryNameForm();

    this.workspaceId = parseInt(this.route.snapshot.paramMap.get('id'));
    this.workspaceName = this.route.snapshot.paramMap.get('name');

    console.log(this.workspaceId);
    this.breadcrumbService.set('users', 'Users');
    this.breadcrumbService.set('users/workspace/:id/:name', this.workspaceName);

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
    this.DirectoryParams.workspaceId = this.workspaceId;
    this.adminService
      .getDirectoriesInWorkspace(this.DirectoryParams)
      .subscribe({
        next: (res) => {
          this.directories = res.data;
          this.totalCount = res.count;
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
