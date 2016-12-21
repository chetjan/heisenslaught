/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { HeroesButtonComponent } from './heroes-button.component';

describe('HeroesButtonComponent', () => {
  let component: HeroesButtonComponent;
  let fixture: ComponentFixture<HeroesButtonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HeroesButtonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HeroesButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
