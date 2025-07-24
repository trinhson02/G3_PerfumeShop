using Microsoft.AspNetCore.Mvc;

namespace G3_PerfumeShop.Controllers
{
    public class MyPostsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
