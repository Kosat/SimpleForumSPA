import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { AuthService } from '../../shared/services/auth.service';
import { IThreadPreviewViewModel } from '../models/IThreadPreviewViewModel';

@Component({
  selector: 'app-thread-settings',
  templateUrl: './thread-settings.component.html',
  styleUrls: ['./thread-settings.component.scss']
})
export class ThreadSettingsComponent implements OnInit {

  private isUserDropdownOpened = false;
  @Input() threadViewModel: IThreadPreviewViewModel;
  @Output() deleteEvent= new EventEmitter<number>();

  constructor(private router: Router, private authService: AuthService) { }

  ngOnInit() {}

  isAdmin(): boolean {
    return this.authService.isLoginAsAdmin();
  }

  isModerator(): boolean {
    return this.authService.isLoginAsModerator();
  }

  toggleDropdown(value: boolean): void {
    this.isUserDropdownOpened = value;
  }

  private edit(): void {
    this.router.navigate(['/forum/thread-create/' + this.threadViewModel.id]);
  }

  private delete(): void {
    this.deleteEvent.emit(this.threadViewModel.id);
  }

}
