import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IDocuments } from '../shared/Models/Documents';
import { DocumentParams } from '../shared/Models/DocumentParams';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { PublicService } from './public.service';
import { ToastrService } from 'ngx-toastr';
import { response } from 'express';
import { DmsService } from '../dms/dms.service';

@Component({
  selector: 'app-public',
  templateUrl: './public.component.html',
  styleUrl: './public.component.scss',
})
export class PublicComponent implements OnInit {
  @ViewChild('search') searchTerm: ElementRef;
  id: number;
  name: string;
  documents: IDocuments[];
  DocumentParams = new DocumentParams();
  totalCount: number;
  selectedDocument: SafeResourceUrl | null = null;
  isPopupVisible: boolean = false;
  sortField: string = '';
  sortOrder: string = '';

  constructor(
    private publicService: PublicService,
    private sanitizer: DomSanitizer,
    private toast: ToastrService,
    private dmsService: DmsService
  ) {}
  ngOnInit(): void {
    this.loadPublicDocuments();
  }

  loadPublicDocuments() {
    this.publicService.getPublicDocuments(this.DocumentParams).subscribe(
      (res) => {
        this.documents = res.data;
        this.totalCount = res.count;
        this.DocumentParams.pageNumber = res.pageNumber;
        this.DocumentParams.pageSize = res.pageSize;
      },
      (error) => {
        console.log(error);
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
    this.DocumentParams.sort = this.sortField + this.sortOrder;
    this.loadPublicDocuments();
  }

  onPageChanged(event: number) {
    this.DocumentParams.pageNumber = event;
    this.loadPublicDocuments();
  }

  OnSearch() {
    this.DocumentParams.search = this.searchTerm.nativeElement.value;
    this.loadPublicDocuments();
  }

  OnReset() {
    this.searchTerm.nativeElement.value = '';
    this.DocumentParams.search = '';
    this.sortField = '';
    this.sortOrder = '';
    this.DocumentParams = new DocumentParams();
    this.loadPublicDocuments();
  }

  closePopup(): void {
    this.isPopupVisible = false;
  }

  onModalBackgroundClick(event: MouseEvent): void {
    // Close the modal if the user clicks outside the modal content
    const target = event.target as HTMLElement;
    if (target.classList.contains('custom-modal')) {
      this.closePopup();
    }
  }

  downloadDocument(id: number, name: string) {
    this.dmsService.downloadDocumentById(id, name).subscribe({
      next: (response) => {
        console.log('Download initiated successfully');
      },
      error: (err) => {
        console.error('Download failed', err);
      },
    });
  }

  previewDocument(id: number): void {
    this.dmsService.previewDocument(id).subscribe({
      next: (url) => {
        this.selectedDocument =
          this.sanitizer.bypassSecurityTrustResourceUrl(url);
        this.isPopupVisible = true;
      },
      error: (err) => {
        this.toast.error('Error previewing document', err);
      },
    });
  }
}
