using Data.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Concrete
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, LibraryContext>, ICategoryRepository
    {
        public void DeleteFromCategory(int productId, int categoryId)
        {
            using (var context = new LibraryContext())
            {
                var book = context.BookCategories
                                  .Where(b => b.BookId == productId && b.CategoryId == categoryId)
                                  .FirstOrDefault();


                context.Remove(book);
                context.SaveChanges();
            }
        }

        public Category GetByIdWithBooks(int categoryId)
        {
            using (var context = new LibraryContext())
            {
                return context.Categories
                              .Where(c => c.Id == categoryId)
                              .Include(c => c.BookCategories)
                              .ThenInclude(c => c.Book)
                              .FirstOrDefault();
            }
        }
    }
}
