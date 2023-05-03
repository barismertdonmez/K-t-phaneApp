using Entity;

namespace KütüphaneApp.ViewModels
{
    public class BookViewModel
    {
        public List<Book> Books { get; set; }
        public List<Category> Categories { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }
        public int TotalPages()
        {
            return (int)Math.Ceiling((decimal)TotalItems / ItemPerPage);
        }
    }
}
