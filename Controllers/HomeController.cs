using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sivatraders.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Sivatraders.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IOptions<APIdata> _appconfig;

        public HomeController(ILogger<HomeController> logger, IOptions<APIdata> settings)
        {
            _logger = logger;
            _appconfig = settings;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<ActionResult> Register(Message msg)
        {
            if (ModelState.IsValid)
            {
                try

                {
                    using (var httpClient = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(new HttpMethod("POST"), _appconfig.Value.url))
                        {
                            request.Headers.TryAddWithoutValidation("Authorization", _appconfig.Value.authorization);

                            request.Content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"to\": \"919442154464\", \"type\": \"template\", \"template\": { \"name\": \"welcome_siva_traders\", \"language\": { \"code\": \"en_US\" } } }");
                            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                            var response = await httpClient.SendAsync(request);
                            return Content("Success");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Content("fail");
                }
            }
            return Content("fail");
        }

                [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}