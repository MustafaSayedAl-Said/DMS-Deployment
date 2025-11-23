namespace DMS.Core.Entities
{
    public class ActionLog
    {
        public int Id { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        public int? UserId { get; set; }

        public int? DocumentId { get; set; }

        public string UserName { get; set; }

        public string DocumentName { get; set; }

        public DateTime CreationDate { get; set; }

        public User User { get; set; }
        public Document Document { get; set; }
    }


    //public class ActionLogEvent : Event
    //{
    //    public int Id { get; set; }


    //    public int? DocumentId { get; set; }

    //    public string UserName { get; set; }

    //    public string DocumentName { get; set; }


    //}


    //public class Event
    //{
    //    public DateTime CreationDate { get; set; }
    //    public int UserId { get; set; }
    //    public int ActionTypeId { get; set; }

    //}

    public enum ActionTypeEnum
    {
        Upload,
        Download,
        Preview
    }
}
