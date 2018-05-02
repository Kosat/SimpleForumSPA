import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-error-page',
  templateUrl: './error-page.component.html'
})
export class ErrorPageComponent implements OnInit {

  @Input() code: number;
  @Input() message: string;
  @Output() action = new EventEmitter();

  constructor(private route: ActivatedRoute) {
    this.code = this.route.snapshot.data.code
      || this.route.snapshot.queryParams.code
      || 500;
    this.message = this.route.snapshot.data.message
      || this.route.snapshot.queryParams.message
      || 'Error occured';
  }

  ngOnInit(): void {
    // tslint:disable-next-line:no-debugger
    // debugger;
  }

  public back($event) {
    window.history.back();
  }

}
