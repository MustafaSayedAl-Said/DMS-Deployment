export interface ActionLog{
    Id: number;
    ActionType: number;
    UserId: number | null;
    DocumentId: number | null;
    UserName: string;
    DocumentName: string;
    CreationDate: Date
}