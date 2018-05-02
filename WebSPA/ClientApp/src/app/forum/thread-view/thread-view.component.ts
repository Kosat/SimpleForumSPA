import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';
import { AuthService } from '../../shared/services/auth.service';
import { Logger } from 'angular2-logger/core';
import { ToastrService } from 'ngx-toastr';
import { IThreadViewModel } from '../models/IThreadViewModel';
import { IThreadReply } from '../models/IThreadReply';
import { ForumService } from '../services/forum.service';

@Component({
  selector: 'app-thread-view',
  templateUrl: './thread-view.component.html',
  styleUrls: ['./thread-view.component.scss']
})
export class ThreadViewComponent implements OnInit, OnDestroy {
  private threadId: number;
  private newReplyText: string;
  private data$: Observable<IThreadViewModel>;
  private replies$: Observable<IThreadReply[]>;
  private replies_: IThreadReply[];
  private ngUnsubscribe = new Subject<void>();

  constructor(
    private logger: Logger,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private forumService: ForumService,
    private authService: AuthService) {
    this.threadId = +this.route.snapshot.data.threadId
      || +this.route.snapshot.params.threadId
      || +this.route.snapshot.queryParams.threadId
      || -1;
  }

  ngOnInit(): void {
    if (this.threadId !== -1) {
      this.data$ = this.forumService.getThreadView(this.threadId)
        .takeUntil(this.ngUnsubscribe);
      this.data$.subscribe(resp => {
        this.replies_ = resp.replies;
        this.replies$ = of(this.replies_);
      });
    }
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  onAddReply(): void {
    const vm: IThreadReply = {
      id: 0,
      threadId: this.threadId,
      userCreatedId: this.authService.UserData.sub,
      userCreatedEmail: this.authService.UserData.email,
      timeCreated: new Date(),
      content: `${this.newReplyText}`
    };
    this.forumService.addReply(vm)
      .subscribe(
        res => {
          this.newReplyText = '';
          this.replies_.push(res);
          const msg = `Created a new reply for thread [${this.threadId}]`;
          this.logger.info(msg);
          this.toastr.success(msg);
        },
        err => {
          const msg = 'Error posting a new reply.';
          this.logger.error(msg);
          this.toastr.error(msg);
        }
      );
  }
}
