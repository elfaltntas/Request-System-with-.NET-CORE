using BusinessLayer.Abstract;
using DataAcces.Abstract;
using DataAcces.Entity_Framework;
using DataAcces.Repository;
using Entity1.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class CategoryManager : ICategoryService
    {
        //EfCategoryRepository efCategoryRepository;

        ICategory _category;
        public CategoryManager(ICategory category)
            
        {
            _category=category;
        }
        public void CategoryAdd(Category category)
        {
            _category.Insert(category);
        }

        public void CategoryDelete(Category category)
        {
            _category.Delete(category);
        }

        public void CategoryUpdate(Category category)
        {
            _category.Update(category);
        }

        public Category GetById(int id)
        {
           return _category.Get(id);
        }

        public List<Category> GetList()
        {
            return _category.GetAll();
        }
    }
}
