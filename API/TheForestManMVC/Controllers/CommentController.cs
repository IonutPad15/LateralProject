using Microsoft.AspNetCore.Mvc;

namespace TheForestManMVC.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CommentsByPostId( Guid id)
        {
            return View();
        }
    }
}
