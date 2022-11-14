using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        Dbcontext db;
        public ProductRepository(Dbcontext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Product product)
        {
           var obj=db.Products.FirstOrDefault(x => x.Id == product.Id);
           if(obj!=null)
            {
                obj.Name = product.Name;
                obj.Description = product.Description;
                obj.price = product.price;
                obj.listprice = product.listprice;
                obj.price50 = product.price50;
                obj.price100 = product.price100;
                obj.ISBN = product.ISBN;
                obj.Author = product.Author;
                obj.CoverTypeId = product.CoverTypeId;
                obj.CategoryId = product.CategoryId;
                obj.Id = product.Id;
                obj.ImageURL = product.ImageURL;
                //anotherway if we want to update particular fields only

            }
        }
    }
}
