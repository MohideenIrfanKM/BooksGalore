using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
	public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
	{
		public readonly Dbcontext db;
		public ApplicationUserRepository(Dbcontext db) : base(db)
		{
			this.db = db;
		}
	}
}
