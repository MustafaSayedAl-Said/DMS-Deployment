import { IUsersGet } from './UsersGet';

export interface IPAginationUsers {
  count: number;
  pageSize: number;
  pageNumber: number;
  data: IUsersGet[];
}
