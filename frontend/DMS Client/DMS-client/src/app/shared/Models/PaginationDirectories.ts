import { IDirectories } from './Directories';

export interface IPaginationDirectories {
  count: number;
  pageSize: number;
  pageNumber: number;
  data: IDirectories[];
}
