/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { TeamHeroPicksComponent } from './team-hero-picks.component';

describe('TeamHeroPicksComponent', () => {
  let component: TeamHeroPicksComponent;
  let fixture: ComponentFixture<TeamHeroPicksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeamHeroPicksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamHeroPicksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
