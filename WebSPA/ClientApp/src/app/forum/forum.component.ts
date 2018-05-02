import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { AuthService } from '../shared/services/auth.service';
import { of } from 'rxjs/observable/of';
import 'rxjs/add/operator/takeUntil';
import 'rxjs/add/observable/empty';
import 'rxjs/add/operator/catch';
import { Logger } from 'angular2-logger/core';
import { ToastrService } from 'ngx-toastr';
import { IThreadPreviewViewModel } from './models/IThreadPreviewViewModel';
import { ForumService } from './services/forum.service';

@Component({
  selector: 'app-forum',
  templateUrl: './forum.component.html',
  styleUrls: ['./forum.component.scss']
})
export class ForumComponent implements OnInit, OnDestroy {
  private data_: IThreadPreviewViewModel[];
  private data$: Observable<IThreadPreviewViewModel[]>;
  private authenticated = false;
  private ngUnsubscribe = new Subject<void>();

  constructor(
    private logger: Logger,
    private toastr: ToastrService,
    private forumService: ForumService,
    private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.userLoadededEvent$.subscribe(user => {
      this.authenticated = user != null;
    });
    this.authenticated = this.authService.IsAuthorized;
    this.forumService.getThreads()
      .takeUntil(this.ngUnsubscribe)
      .subscribe(data => { this.data_ = data; this.data$ = of(this.data_); });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  isModerator(): boolean {
    return this.authService.isLoginAsModerator();
  }

  onDeleteThread(threadId: number): void {
    this.forumService.deleteThread(threadId)
      .subscribe(
        res => {
          const threadVm: IThreadPreviewViewModel = this.data_.filter(t => t.id === threadId)[0];
          const threadIdx: number = this.data_.indexOf(threadVm);
          if (threadIdx !== -1) {
              this.data_.splice(threadIdx, 1);
          }
          const msg = `Deleted a thread [${threadId}]`;
          this.logger.info(msg);
          this.toastr.success(msg);
        },
        err => {
          const msg = `Error deleting thread [${threadId}].`;
          this.logger.error(msg);
          this.toastr.error(msg);
        }
      );
  }

}
