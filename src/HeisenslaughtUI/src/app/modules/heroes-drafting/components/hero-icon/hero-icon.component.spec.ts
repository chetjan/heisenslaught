/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { HeroIconComponent } from './hero-icon.component';

describe('HeroIconComponent', () => {
  let component: HeroIconComponent;
  let fixture: ComponentFixture<HeroIconComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HeroIconComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HeroIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
