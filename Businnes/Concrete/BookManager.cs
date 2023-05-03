using Businnes.Abstract;
using Data.Abstract;
using Data.DataModels;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Businnes.Concrete
{
    public class BookManager : IBookService
    {
        private IBookRepository _bookRepository;
        public BookManager(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public void Create(Book entity)
        {
            _bookRepository.Create(entity);
        }

        public void CreateBookWithCategory(Book book, List<int> categoryIds) 
        {
            _bookRepository.CreateBookWithCategory(book, categoryIds);
        }

        public void Delete(Book entity)
        {
            _bookRepository.Delete(entity);
        }

        public List<Book> GetAll()
        {
            return _bookRepository.GetAll();    
        }

        public DataBookDetailsModel GetBookDetails(int id)
        {
            return _bookRepository.GetBookDetails(id);
        }

        public List<Book> GetBooksByCategoryName(string name, int page, int pageSize)
        {
            return _bookRepository.GetBooksByCategoryName(name, page, pageSize);
        }

        public Book GetById(int id)
        {
            return _bookRepository.GetById(id);
        }

        public Book GetByIdWithCategories(int id)
        {
            return _bookRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return _bookRepository.GetCountByCategory(category);
        }

        public List<Book> GetSearchResult(string searchString)
        {
            return _bookRepository.GetSearchResult(searchString);
        }

        public void Update(Book entity)
        {
            _bookRepository.Update(entity);
        }

        public void Update(Book entity, int[] categoryIds)
        {
            _bookRepository.Update(entity, categoryIds);
        }
    }
}
