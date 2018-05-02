import { IThreadReply } from './IThreadReply';
import { IThreadPreviewViewModel } from './IThreadPreviewViewModel';

export interface IThreadViewModel extends IThreadPreviewViewModel {
  timeCreated: Date;
  content: string;
  replies: IThreadReply[];
}
