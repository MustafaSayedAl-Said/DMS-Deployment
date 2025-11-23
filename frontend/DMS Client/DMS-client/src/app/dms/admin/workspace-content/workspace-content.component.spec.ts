import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkspaceContentComponent } from './workspace-content.component';

describe('WorkspaceContentComponent', () => {
  let component: WorkspaceContentComponent;
  let fixture: ComponentFixture<WorkspaceContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WorkspaceContentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkspaceContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
