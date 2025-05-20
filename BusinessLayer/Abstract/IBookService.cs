using Entity1.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IBookService
    {
        void BookAdd(Books book);
        void BookDelete(Books book);
        void BookUpdate(Books book);
        List<Books> GetAll();
        Books GetById(int id);
        List<Books> GetBooksListWithCategory();
        List<Books> GetBooksListWithWriter();
    }
}
