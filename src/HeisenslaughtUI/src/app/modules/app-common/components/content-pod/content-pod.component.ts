import { Component, OnInit, Input, ContentChild, ViewChild, AfterContentInit, TemplateRef, AfterViewInit } from '@angular/core';

@Component({
  selector: 'content-pod',
  templateUrl: './content-pod.component.html',
  styleUrls: ['./content-pod.component.scss']
})
export class ContentPodComponent implements OnInit, AfterViewInit {

  @Input()
  public title: string;

  @ViewChild('testRef', { read: TemplateRef })
  public actions: TemplateRef<any>;

  constructor() { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    console.log('actions', this.actions);
  }
}
