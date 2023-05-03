using Entity;

namespace KütüphaneApp.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public List<BookCategory> BookCategories { get; set; }
        public List<Book> Books { get; set; }
    }
}
