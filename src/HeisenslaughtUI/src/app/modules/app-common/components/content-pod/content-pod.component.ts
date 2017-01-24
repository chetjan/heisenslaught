import { Component, OnInit, Input, ContentChild, ViewChild, AfterContentInit, TemplateRef, AfterViewInit, ElementRef, ContentChildren, QueryList } from '@angular/core';
import {MdButton} from '@angular/material/button'

@Component({
  selector: 'content-pod',
  templateUrl: './content-pod.component.html',
  styleUrls: ['./content-pod.component.scss']
})
export class ContentPodComponent implements OnInit, AfterViewInit, AfterContentInit {

  @Input()
  public title: string;

  @ContentChildren('actions')
  public actions: QueryList<ElementRef>;

  constructor() { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    console.log('actions', this.actions);
  }

  ngAfterContentInit() {
    console.log('actions', this.actions);
  }
}
