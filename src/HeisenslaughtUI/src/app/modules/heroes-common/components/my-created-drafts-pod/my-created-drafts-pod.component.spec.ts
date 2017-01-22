/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MyCreatedDraftsPodComponent } from './my-created-drafts-pod.component';

describe('MyCreatedDraftsPodComponent', () => {
  let component: MyCreatedDraftsPodComponent;
  let fixture: ComponentFixture<MyCreatedDraftsPodComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MyCreatedDraftsPodComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyCreatedDraftsPodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
