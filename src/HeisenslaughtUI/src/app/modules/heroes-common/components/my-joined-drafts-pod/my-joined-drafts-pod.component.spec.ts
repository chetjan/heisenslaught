/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MyJoinedDraftsPodComponent } from './my-joined-drafts-pod.component';

describe('MyJoinedDraftsPodComponent', () => {
  let component: MyJoinedDraftsPodComponent;
  let fixture: ComponentFixture<MyJoinedDraftsPodComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MyJoinedDraftsPodComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyJoinedDraftsPodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
