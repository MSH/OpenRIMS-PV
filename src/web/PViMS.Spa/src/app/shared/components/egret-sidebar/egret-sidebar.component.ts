import {
  Component,
  OnInit,
  Input,
  HostBinding,
  OnDestroy,
  HostListener,
  Directive,
  Renderer2,
  ElementRef,
  ChangeDetectorRef
} from "@angular/core";
import { MatchMediaService } from "app/shared/services/match-media.service";
import { MediaObserver } from "@angular/flex-layout";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { EgretSidebarHelperService } from "./egret-sidebar-helper.service";

@Component({
  selector: "egret-sidebar",
  templateUrl: "./egret-sidebar.component.html",
  styleUrls: ["./egret-sidebar.component.scss"]
})
export class EgretSidebarComponent implements OnInit, OnDestroy {
  // Name
  @Input()
  name: string;

  // right
  @Input()
  @HostBinding("class.position-right")
  right: boolean;

  // Open
  @HostBinding("class.open")
  opened: boolean;

  @HostBinding("class.sidebar-locked-open")
  sidebarLockedOpen: boolean;

  //mode
  @HostBinding("class.is-over")
  isOver: boolean;

  private backdrop: HTMLElement | null = null;

  private lockedBreakpoint = "gt-sm";
  private unsubscribeAll: Subject<any>;

  constructor(
    private matchMediaService: MatchMediaService,
    private mediaObserver: MediaObserver,
    private sidebarHelperService: EgretSidebarHelperService,
    private _renderer: Renderer2,
    private _elementRef: ElementRef,
    private cdr: ChangeDetectorRef
  ) {
    this.unsubscribeAll = new Subject();
  }

  ngOnInit() {
    this.sidebarHelperService.setSidebar(this.name, this);

    if (this.mediaObserver.isActive(this.lockedBreakpoint)) {
      this.sidebarLockedOpen = true;
      this.opened = true;
    } else {
      this.sidebarLockedOpen = false;
      this.opened = false;
    }

    this.matchMediaService.onMediaChange
      .pipe(takeUntil(this.unsubscribeAll))
      .subscribe(() => {
        // console.log("medua sub");
        if (this.mediaObserver.isActive(this.lockedBreakpoint)) {
          this.sidebarLockedOpen = true;
          this.opened = true;
        } else {
          this.sidebarLockedOpen = false;
          this.opened = false;
        }
      });
  }

  open() {
    this.opened = true;
    if (!this.sidebarLockedOpen && !this.backdrop) {
      this.showBackdrop();
    }
  }

  close() {
    this.opened = false;
    this.hideBackdrop();
  }

  toggle() {
    if (this.opened) {
      this.close();
    } else {
      this.open();
    }
  }

  showBackdrop() {
    this.backdrop = this._renderer.createElement("div");
    this.backdrop.classList.add("egret-sidebar-overlay");

    this._renderer.appendChild(
      this._elementRef.nativeElement.parentElement,
      this.backdrop
    );

    // Close sidebar onclick
    this.backdrop.addEventListener("click", () => {
      this.close();
    });

    this.cdr.markForCheck();
  }

  hideBackdrop() {
    if (this.backdrop) {
      this.backdrop.parentNode.removeChild(this.backdrop);
      this.backdrop = null;
    }

    this.cdr.markForCheck();
  }

  ngOnDestroy(): void {
    this.unsubscribeAll.next();
    this.unsubscribeAll.complete();
    this.sidebarHelperService.removeSidebar(this.name);
  }
}

@Directive({
  selector: "[egretSidebarToggler]"
})
export class EgretSidebarTogglerDirective {
  @Input("egretSidebarToggler")
  public id: any;

  constructor(private egretSidebarHelperService: EgretSidebarHelperService) {}

  @HostListener("click")
  onClick() {
    this.egretSidebarHelperService.getSidebar(this.id).toggle();
  }
}
