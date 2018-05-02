import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Subject } from 'rxjs/Subject';
import { Logger } from 'angular2-logger/core';
import { IUserViewModel } from './models/IUserViewModel';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit, OnDestroy {

  private userslist: IUserViewModel[] = null;
  private ngUnsubscribe = new Subject<void>();

  constructor(private logger: Logger, private http: HttpClient) {}

  ngOnInit() {
    this.http
      .get<IUserViewModel[]>('account/users')
      .takeUntil(this.ngUnsubscribe)
      .subscribe(data => { this.userslist = data; });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}


