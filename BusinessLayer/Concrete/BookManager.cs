using BusinessLayer.Abstract;
using DataAcces.Abstract;
using DataAcces.Entity_Framework;
using Entity1.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class BookManager : IBookService
    {
        IBook _book;

       

        public BookManager(IBook book)
        {
            _book = book;
        }

        public void BookAdd(Books books)
        {
            throw new NotImplementedException();
        }

        public void BookDelete(Books books)
        {
            throw new NotImplementedException();
        }

        public void BookUpdate(Books books)
        {
            throw new NotImplementedException();
        }

        public List<Books> GetAll()
        {
            return _book.GetAll();
        }

       

        public List<Books> GetBooksListWithCategory()
        {
            return _book.GetListWithCategory();
        }

        public List<Books> GetBooksListWithWriter()
        {
            return _book.GetListWithWriter();
        }

        public Books GetById(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
