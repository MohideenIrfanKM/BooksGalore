using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksGalore.Db;

namespace BooksGalore.Utility
{
	public interface IDbInitializer
	{
		void Initialize();
	}
}
