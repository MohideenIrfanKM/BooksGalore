
using System.Security.Claims;
using BooksGalore.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksGalore.ViewComponent
{
    public class ShoppingCartViewComponent: Microsoft.AspNetCore.Mvc.ViewComponent
    {
        public readonly IUnitofWork db;

        public ShoppingCartViewComponent(IUnitofWork db)
        {
            this.db=db; 
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var x = (ClaimsIdentity)User.Identity;
            var claim=x.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                if (HttpContext.Session.GetInt32(Utility.Util.SessionCart) != null)
                    return View(HttpContext.Session.GetInt32(Utility.Util.SessionCart));//if session is already made we are getting it or otherwise first we set it and then get it

                else
                {
                    HttpContext.Session.SetInt32(Utility.Util.SessionCart, db.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
                    HttpContext.Session.GetInt32(Utility.Util.SessionCart);//if session is already made we are getting it or otherwise first we set it and then get it

                    return View(HttpContext.Session.GetInt32(Utility.Util.SessionCart));//if session is already made we are getting it or otherwise first we set it and then get it

                }

            }
            else
            {
                //this condition is for if the user is logged out or not log in
                HttpContext.Session.Clear();//clearing the sessions if it is made
                return View(0);
            }
        }
    }
}
