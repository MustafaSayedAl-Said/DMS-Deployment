namespace DMS.Core.Sharing
{
    public class UserParams
    {
        public int MaxPageSize { get; set; } = 15;

        private int _pageSize = 6;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

        public string Sort { get; set; }

        public int PageNumber { get; set; } = 1;

        public string Search { get; set; }
    }
}
