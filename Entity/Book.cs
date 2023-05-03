using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Book
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string WriterName { get; set; }
        public int TotalPage { get; set; }
        public string ImageUrl { get; set; }
        public List<BookCategory> BookCategories { get; set; }

    }
}
