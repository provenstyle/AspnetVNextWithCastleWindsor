using Castle.Core.Logging;
using Microsoft.AspNet.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageService _messageService;

        public HomeController(IMessageService messageService)
        {
            _messageService = messageService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IActionResult Index()
        {
            return View(new HomeModel
            {
                Message = _messageService.FormatMessage("Craig")
            });
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}