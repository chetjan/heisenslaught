import { Component, OnInit, Input, ViewContainerRef, TemplateRef, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import {
  Overlay, OriginConnectionPosition,
  OverlayConnectionPosition, OverlayState, OverlayRef,
  TemplatePortal
} from '@angular/material/core';

@Component({
  selector: 'app-tooltip',
  templateUrl: './html-tooltip.component.html',
  styleUrls: ['./html-tooltip.component.scss']
})
export class HtmlTooltipComponent implements OnInit, OnDestroy {

  private _attachement: Element;
  private _overlayRef: OverlayRef;
  private _eventBindings: { type: string, listener: EventListener }[] = [];

  @Input()
  public get attachement(): Element {
    return this._attachement ? this._attachement : (<Element>this._elementRef.nativeElement).parentElement;
  }

  public set attachement(value: Element) {
    this._attachement = value;
  }

  @ViewChild('tplRef', { read: TemplateRef })
  private _tplRef: TemplateRef<any>;


  constructor(
    private _overlay: Overlay,
    private _viewContainerRef: ViewContainerRef,
    private _elementRef: ElementRef
  ) {


  }

  private createTooltip() {
    this._createOverlay();
    let portal = new TemplatePortal(this._tplRef, this._viewContainerRef);
    this._overlayRef.attach(portal);
  }

  private _createOverlay() {
    let origin = this._getOrigin();
    let position = this._getPosition();
    let strategy = this._overlay.position().connectedTo({ nativeElement: this.attachement }, origin, position);
    let config = new OverlayState();
    config.positionStrategy = strategy;
    config.hasBackdrop = false;
    this._overlayRef = this._overlay.create(config);
    this._overlayRef.overlayElement.style.pointerEvents = 'none';
  }

  private _getOrigin(): OriginConnectionPosition {
    return {
      originX: 'end',
      originY: 'top'
    };
  }

  private _getPosition(): OverlayConnectionPosition {
    return {
      overlayX: 'start',
      overlayY: 'top'
    };
  }

  public get isVisible(): boolean {
    return false;
  }

  ngOnInit() {
    let att = this.attachement;

    let listener = this.onMouseEnter.bind(this);
    att.addEventListener('mouseenter', listener);
    this._eventBindings.push({ type: 'mouseenter', listener: listener });

    listener = this.onMouseLeave.bind(this);
    att.addEventListener('mouseleave', listener);
    this._eventBindings.push({ type: 'mouseleave', listener: listener });
  }

  public ngOnDestroy() {
    this.removeEvents();
    this.dispose();
  }

  private dispose(): void {
    if (this._overlayRef) {
      this._overlayRef.dispose();
      this._overlayRef = null;
    }
  }

  private removeEvents(): void {
    let att = this.attachement;
    this._eventBindings.forEach((binding) => {
      att.removeEventListener(binding.type, binding.listener);
    });
  }

  private onMouseEnter(): void {
    if (!this._overlayRef) {
      this.createTooltip();
    }
  }

  private onMouseLeave(): void {
    this.dispose();
  }

}
