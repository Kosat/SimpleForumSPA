import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Md5 } from 'ts-md5/dist/md5';
import { Logger } from 'angular2-logger/core';

@Component({
  selector: 'app-gravatar',
  template: '<img [src]="uri" [ngClass]="{circle: isCircled}" alt="avatar">',
  styles: [' .circle {border-radius: 50%;}']
})
export class GravatarComponent implements OnInit, OnChanges {
  @Input() email = '';
  @Input() size = '62';
  @Input() isCircled = false;
  private readonly defaultImage = 'identicon';
  private uri = '';

  constructor(private logger: Logger) {}

  ngOnInit() {
    this.setUri(this.email);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['email']) {
      this.setUri(changes['email'].currentValue);
    }
  }

  setUri(email: string) {
    if (email) {
      this.uri = '//www.gravatar.com/avatar/' +
        Md5.hashStr(this.email.trim()) +
        '?s=' + this.size +
        '&d=' + this.defaultImage;
    }
  }

}
