namespace BCBlog.Client.Models
{
    public class PagedList<T> where T : class
    {
        public int Page {  get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<T> Data { get; set; } = [];


    }
}
