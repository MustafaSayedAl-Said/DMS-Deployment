import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IDirectories } from '../../shared/Models/Directories';
import { DmsService } from '../dms.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../../core/confirmation-dialog/confirmation-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../../account/account.service';

@Component({
  selector: 'app-directory',
  templateUrl: './directory.component.html',
  styleUrl: './directory.component.scss',
})
export class DirectoryComponent implements OnInit {
  @Input() directory: IDirectories;
  editMode = false;
  directoryNameForm: FormGroup;
  fromUsersTab: boolean;
  workspaceId: number;
  workspaceName: string;
  fromUsers: boolean = false;
  @Output() nameUpdated: EventEmitter<{ id: number; newName: string }> =
    new EventEmitter();

  @Output() directoryDeleted: EventEmitter<number> = new EventEmitter();

  constructor(
    private dmsService: DmsService,
    private fb: FormBuilder,
    public dialog: MatDialog,
    private toast: ToastrService,
    private router: Router,
    private route: ActivatedRoute,
    private accountService: AccountService
  ) {}
  ngOnInit(): void {
    this.createDirectoryNameForm();
    this.route.queryParams.subscribe((params) => {
      this.fromUsersTab = params['fromUsers'] === 'true';
      this.workspaceId = +this.route.snapshot.paramMap.get('id');
      this.workspaceName = this.route.snapshot.paramMap.get('name');
      if (this.workspaceName !== null) {
        this.fromUsers = true;
      }
      // this.accountService.getWorkspaceNameFromSubject().subscribe((workspaceName:string) => {
      //   if(workspaceName !== this.workspaceName){
      //     this.fromUsers = true;
      //   }
      // })
      console.log(this.workspaceId);
    });
  }

  createDirectoryNameForm() {
    this.directoryNameForm = this.fb.group({
      name: ['', [Validators.required]],
    });
  }

  get _name() {
    return this.directoryNameForm.get('name');
  }
  toggleEdit() {
    this.directoryNameForm.setValue({ name: this.directory.name });

    this.editMode = !this.editMode;
  }

  saveEdit() {
    const newName = this._name.value;
    console.log(newName);
    this.dmsService.updateDirectoryName(this.directory.id, newName).subscribe({
      next: () => {
        this.editMode = false;
        this.toast.success('Name updated successfully');

        // Emit the new name to the parent component
        this.nameUpdated.emit({ id: this.directory.id, newName });
      },
      error: (err) => {
        this.editMode = false;
        this.toast.error('Error updating name', err);
      },
    });
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.deleteDirectory();
      }
    });
  }

  deleteDirectory() {
    this.dmsService.deleteDirectory(this.directory.id).subscribe({
      next: () => {
        this.toast.success('Directory deleted successfully');
        this.directoryDeleted.emit(this.directory.id);
      },
      error: (err) => {
        this.toast.error('Error deleting directory', err);
      },
    });
  }
}
