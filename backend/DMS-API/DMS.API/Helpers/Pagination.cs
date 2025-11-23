namespace DMS.API.Helpers
{
    public class Pagination<T> where T : class
    {
        public Pagination(int count, int pageSize, int pageNumber, IReadOnlyList<T> data)
        {
            Count = count;
            PageSize = pageSize;
            PageNumber = pageNumber;
            Data = data;
        }

        public int Count { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public IReadOnlyList<T> Data { get; set; }
    }
}
