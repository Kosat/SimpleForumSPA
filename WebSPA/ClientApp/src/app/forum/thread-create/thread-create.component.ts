import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../shared/services/auth.service';
import { NgModel } from '@angular/forms';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { Logger } from 'angular2-logger/core';
import { Toast, ToastrService } from 'ngx-toastr';
import { IThreadPreviewViewModel } from '../models/IThreadPreviewViewModel';
import { ForumService } from '../services/forum.service';

@Component({
  selector: 'app-thread-create',
  templateUrl: './thread-create.component.html',
  styleUrls: ['./thread-create.component.scss']
})
export class ThreadCreateComponent implements OnInit, OnDestroy {

  private data: IThreadPreviewViewModel = null;
  private threadId: number;
  private ngUnsubscribe = new Subject<void>();

  constructor(
    private logger: Logger,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    private forumService: ForumService,
    private authService: AuthService) {
      this.threadId = this.route.snapshot.data.threadId
      || +this.route.snapshot.params.threadId
      || +this.route.snapshot.queryParams.threadId
      || -1;
  }

  ngOnInit(): void {
    if (this.threadId !== -1) {
      // fetch existing Thread from server
      this.forumService.getThread(this.threadId)
      .takeUntil(this.ngUnsubscribe)
      .subscribe(data => { this.data = data; });
    }
    else {
      // create a briend-new thread
      this.data = <IThreadPreviewViewModel> {
        id: 0,
        userCreatedId: this.authService.UserData.sub,
        userCreatedEmail: this.authService.UserData.email,
        repliesCount: 0
      };
    }
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  private onCreateOrEditThread() {
    const navigateOut = () => this.router.navigate(['/forum']);
    if (this.threadId === -1) {
      this.forumService.createNewThread(this.data)
      .subscribe(
        res => {
          const msg = `Created new thread [${res.id}]`;
          this.logger.info(msg);
          this.toastr.success(msg);
          navigateOut();
        },
        err => {
          const msg = 'Error posting new thread.';
          this.logger.error(msg);
          this.toastr.error(msg);
        }
      );
    }
    else {
      this.forumService.editThread(this.data)
      .subscribe(
        res => {
          const msg = `Edited thread [${res.id}].`;
          this.logger.info(msg);
          this.toastr.success(msg);
          navigateOut();
        },
        err => {
          const msg = `Error editing thread.`;
          this.logger.error(msg);
          this.toastr.error(msg);
        }
      );
    }
  }
}


