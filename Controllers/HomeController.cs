using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sivatraders.Models;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;


namespace Sivatraders.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IOptions<APIdata> _appconfig;
        const string SPREADSHEET_ID = "19UtE5h3ctselVomPXTb0CWYoVE0TBm7ivWyukQxJSAU";
        const string SHEET_NAME = "Contact";
        SpreadsheetsResource.ValuesResource _googleSheetValues;

        public HomeController(ILogger<HomeController> logger, IOptions<APIdata> settings, GoogleSheetsHelper googleSheetsHelper)
        {
            _logger = logger;
            _appconfig = settings;
            _googleSheetValues = googleSheetsHelper.Service.Spreadsheets.Values;
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
                    var range = $"{SHEET_NAME}!A:D";
                    ValueRange valueRange = new ValueRange();
                    valueRange.MajorDimension = "ROWS";
                    var oblist = new List<object>() { msg.name, msg.email, msg.mobilenumber, msg.message };
                    valueRange.Values = new List<IList<object>> { oblist };
                    var appendRequest = _googleSheetValues.Append(valueRange, SPREADSHEET_ID, range);
                    appendRequest.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;
                    appendRequest.Execute();

                    using (var httpClient = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(new HttpMethod("POST"), _appconfig.Value.url))
                        {
                            request.Headers.TryAddWithoutValidation("Authorization", _appconfig.Value.authorization);

                            //  request.Content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"to\": \"919442154464\", \"type\": \"template\", \"template\": { \"name\": \"welcome_siva_traders\", \"language\": { \"code\": \"en_US\" } } }");
                            request.Content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"to\":  \"" + msg.mobilenumber + "\", \"type\": \"template\", \"template\": { \"name\": \"welcome_siva_traders\", \"language\": { \"code\": \"en_US\" } } }");
                            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                            var response = await httpClient.SendAsync(request);
                            await Alertmessage();

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
        public  async Task<string>  Alertmessage()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), _appconfig.Value.url))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", _appconfig.Value.authorization);

                        //  request.Content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"to\": \"919442154464\", \"type\": \"template\", \"template\": { \"name\": \"welcome_siva_traders\", \"language\": { \"code\": \"en_US\" } } }");
                        request.Content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"to\":\"919738232707\", \"type\": \"template\", \"template\": { \"name\": \"contact_alert_siva_traders\", \"language\": { \"code\": \"en_US\" } } }");
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = await httpClient.SendAsync(request);

                        return "Success";                                                                                                            
                    }
                }
            }
            catch (Exception ex) {
                return "fail";
            }
        }

                [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}