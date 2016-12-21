/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { NotFoundScreenComponent } from './not-found-screen.component';

describe('NotFoundScreenComponent', () => {
  let component: NotFoundScreenComponent;
  let fixture: ComponentFixture<NotFoundScreenComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NotFoundScreenComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NotFoundScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
