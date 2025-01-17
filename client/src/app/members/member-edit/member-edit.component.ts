import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit{
  @ViewChild('editProfileForm') editProfileForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){
    if(this.editProfileForm?.dirty){
      $event.returnValue = true;
    }
  }
  member: Member | undefined;
  user: User | null =  null;

  constructor(private accountService: AccountService, private membersService: MembersService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    if(!this.user) return;

    this.membersService.getMember(this.user.userName).subscribe({
      next: member => this.member = member
    })
  }

  updateMember(){
    this.membersService.updateMember(this.editProfileForm?.value).subscribe({
      next: _ => {
        this.toastr.success('Profile updated successfully');
        this.editProfileForm?.reset(this.member);
      }
    })
  }

}
