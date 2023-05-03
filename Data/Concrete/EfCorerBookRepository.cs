using Data.Abstract;
using Data.DataModels;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Concrete
{
    public class EfCorerBookRepository : EfCoreGenericRepository<Book, LibraryContext>, IBookRepository
    {
        public void CreateBookWithCategory(Book book, List<int> categoryIds)
        {
            using (var context = new LibraryContext())
            {
                context.Add(book);
                context.SaveChanges();

                foreach (var item in categoryIds)
                {
                    var bookCategory = new BookCategory()
                    {
                        BookId = book.Id,
                        CategoryId = item,
                    };
                    context.Add(bookCategory);
                }
                context.SaveChanges();
            };
        }

        public DataBookDetailsModel GetBookDetails(int id)
        {
            using (var context = new LibraryContext()) 
            {
                var bookDetails = context.Books
                                         .Where(p => p.Id == id)
                                         .Include(p => p.BookCategories)
                                         .ThenInclude(p => p.Category)
                                         .FirstOrDefault();

                return new DataBookDetailsModel
                {
                    Book = bookDetails,
                    Categories = bookDetails.BookCategories.Select(p => p.Category).ToList(),
                };
            }
        }

        public List<Book> GetBooksByCategoryName(string name, int page, int pageSize)
        {
            using(var context = new LibraryContext())
            {
                var Books = context.Books.AsQueryable();
                if (!string.IsNullOrEmpty(name))
                {
                    Books = Books
                            .Include(p => p.BookCategories)
                            .ThenInclude(p => p.Category)
                            .Where(p => p.BookCategories.Any(a => a.Category.CategoryName.ToLower() == name.ToLower()));
                }
                return Books.Skip((page-1)*pageSize).Take(pageSize).ToList();
                                   
            }
        }

        public Book GetByIdWithCategories(int id)
        {
            using (var context = new LibraryContext())
            {
                return context.Books
                              .Where(b => b.Id == id)
                              .Include(b => b.BookCategories)
                              .ThenInclude(b => b.Category)
                              .FirstOrDefault();
            }
        }

        public int GetCountByCategory(string category)
        {
            using (var context = new LibraryContext())
            {
                var Books = context.Books.AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    Books = Books
                            .Include(p => p.BookCategories)
                            .ThenInclude(p => p.Category)
                            .Where(p => p.BookCategories.Any(a => a.Category.CategoryName.ToLower() == category.ToLower()));
                }
                return Books.Count();
            }
        }

        public List<Book> GetSearchResult(string searchString)
        {
            using (var context = new LibraryContext())
            {
                var search = context.Books
                                    .Where(p => p.BookName.ToLower().Contains(searchString) || p.WriterName.ToLower().Contains(searchString))
                                    .AsQueryable();
                return search.ToList();
            }
        }

        public void Update(Book entity, int[] categoryIds)
        {
            using (var context = new LibraryContext())
            {
                var book = context.Books
                                   .Include(p => p.BookCategories)
                                   .FirstOrDefault(p => p.Id == entity.Id);
                if (book != null)
                {
                    book.BookName = entity.BookName;
                    book.WriterName = entity.WriterName;
                    book.ImageUrl = entity.ImageUrl;
                    book.TotalPage = entity.TotalPage;
                    book.BookCategories = categoryIds.Select(catId => new BookCategory()
                    {
                        BookId = entity.Id,
                        CategoryId = catId,
                    }).ToList();
                    context.SaveChanges();
                }
            }
        }
    }
}
