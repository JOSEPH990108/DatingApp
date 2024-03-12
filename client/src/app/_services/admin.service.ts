import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/user-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.put<string[]>(this.baseUrl + 'admin/edit-user-role/'
      + username  + '?roles=' + roles, {});
  }
}
