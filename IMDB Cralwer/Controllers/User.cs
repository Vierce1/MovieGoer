using Microsoft.AspNetCore.Mvc;

namespace IMDB_Cralwer.Controllers
{
    public class User : Controller
    {
        public IActionResult Index()
        {
            //get session info
            var user = HttpContext.Session.GetString("UserSession");

            return View(User);
        }
    }
}
