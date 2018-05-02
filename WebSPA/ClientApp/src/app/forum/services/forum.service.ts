import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/map';
import { Logger } from 'angular2-logger/core';
import { IThreadPreviewViewModel } from '../models/IThreadPreviewViewModel';
import { IThreadViewModel } from '../models/IThreadViewModel';
import { IThreadReply } from '../models/IThreadReply';


@Injectable()
export class ForumService {
    private settingsLoadedSource = new Subject();
    settingsLoaded$ = this.settingsLoadedSource.asObservable();
    isReady = false;

    constructor(private logger: Logger, private http: HttpClient) { }

    getThreads() {
      return this.http.get<IThreadPreviewViewModel[]>('/forum/threads');
    }

    getThread(threadId: number) {
      return this.http.get<IThreadPreviewViewModel>('/forum/threads/' + threadId);
    }

    createNewThread(threadVm: IThreadPreviewViewModel) {
      return this.http.post<IThreadPreviewViewModel>('/forum/threads', threadVm);
    }

    editThread(threadVm: IThreadPreviewViewModel) {
      return this.http.put<IThreadPreviewViewModel>('/forum/threads', threadVm);
    }

    deleteThread(threadId: number) {
      return this.http.delete<number>('/forum/threads/' + threadId);
    }

    getThreadView(threadId: number) {
      return this.http.get<IThreadViewModel>('/forum/threads/view/' + threadId);
    }

    addReply(replyVm: IThreadReply) {
      return this.http.post<IThreadReply>('/forum/threads/reply', replyVm);
    }
}
