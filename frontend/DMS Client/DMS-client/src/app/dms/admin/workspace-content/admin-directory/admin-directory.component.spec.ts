import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDirectoryComponent } from './admin-directory.component';

describe('AdminDirectoryComponent', () => {
  let component: AdminDirectoryComponent;
  let fixture: ComponentFixture<AdminDirectoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AdminDirectoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminDirectoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
