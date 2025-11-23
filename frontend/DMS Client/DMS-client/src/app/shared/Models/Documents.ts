export interface IDocuments {
  id: number
  name: string
  directoryId: number
  documentContent: string
  modifyDate: string
  isPublic: boolean
  isDeleted: boolean
  ownerName: string
}