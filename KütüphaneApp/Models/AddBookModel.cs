using Entity;

namespace KütüphaneApp.Models
{
    public class AddBookModel
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string WriterName { get; set; }
        public int TotalPage { get; set; }
        public string ImageUrl { get; set; }
        public List<BookCategory> BookCategories { get; set; }
        public List<Category> Categories { get; set; }
    }
}
