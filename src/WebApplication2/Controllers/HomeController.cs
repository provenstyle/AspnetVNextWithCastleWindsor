
namespace WebApplication2.Controllers
{
    using Castle.Core.Logging;
    using MediatR;
    using Microsoft.AspNet.Mvc;
    using Models;
    using Handlers;

    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
            Logger    = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IActionResult Index()
        {
            var pingRespone = _mediator.Send(new Ping());

            return View(new HomeModel
            {
                Message = "Hello " + pingRespone.Message
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