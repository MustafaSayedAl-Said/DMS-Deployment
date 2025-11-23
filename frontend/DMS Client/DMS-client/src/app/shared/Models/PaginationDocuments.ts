import { IDocuments } from "./Documents"

export interface IPaginationDocuments{
    count:number
    pageSize: number
    pageNumber: number
    data: IDocuments[]
}