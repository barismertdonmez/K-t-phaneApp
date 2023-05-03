using Data.DataModels;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Abstract
{
    public interface IBookRepository : IRepository<Book>
    {
        DataBookDetailsModel GetBookDetails(int id);
        List<Book> GetSearchResult(string searchString);
        int GetCountByCategory(string category);
        List<Book> GetBooksByCategoryName(string name, int page, int pageSize);
        void CreateBookWithCategory(Book book, List<int> categoryIds);
        Book GetByIdWithCategories(int id);
        void Update(Book entity, int[] categoryIds);
    }
}
