using Entity;

namespace KütüphaneApp.Models
{
    public class UpdateBookModel
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string WriterName { get; set; }
        public int TotalPage { get; set; }
        public string ImageUrl { get; set; }
        public List<BookCategory> BookCategories{ get; set; }
        public List<Category> SelectedCategories{ get; set; }
        public List<Category> Categories{ get; set; }
    }
}
