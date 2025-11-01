using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using yogloansdotnet.Models; // ✅ Import your models

namespace yogloansdotnet.Controllers
{
    public class UPIPaymentController : Controller
    {
        private readonly string pa = "yogloans@idfcbank";
        private readonly string cu = "INR";

        public IActionResult Index(string transID)
        {
            if (string.IsNullOrEmpty(transID))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(model: transID);
        }

        [HttpPost]
        public async Task<IActionResult> googlepay(string loan_no, string transId, string amount)
        {
            if (string.IsNullOrEmpty(loan_no) || string.IsNullOrEmpty(transId) || string.IsNullOrEmpty(amount))
            {
                return BadRequest("Missing required parameters.");
            }
            string pa = "yogloans@idfcbank";
            string pn = loan_no;
            string tr = transId;
            string tn = "EMIPayment";
            string am = amount;

            var req_upi = new Request_upiIntent
            {
                upiIntentURL = $"gpay://upi/pay?pa={pa}&pn={pn}&tr={tr}&tn={tn}&am={am}&cu=INR"
            };


            var res_link = await StartAsyn_log(req_upi);


            if (res_link.Short_tinyLink.StartsWith("Error") || res_link.Short_tinyLink.StartsWith("Exception"))
            {
                return Content($"Error generating link: {res_link.Short_tinyLink}");
            }

            return Redirect(res_link.Short_tinyLink);
        }

        [HttpPost]
        public async Task<IActionResult> phonepay(string loan_no, string transId, string amount)
        {
            if (string.IsNullOrEmpty(loan_no) || string.IsNullOrEmpty(transId) || string.IsNullOrEmpty(amount))
            {
                return BadRequest("Missing required parameters.");
            }
            string pa = "yogloans@idfcbank";
            string pn = loan_no;
            string tr = transId;
            string tn = "EMIPayment";
          
            string tid = "IDFCQPD1071931792705202474055401314";
            string am = amount;
           string mc = "7322";
           string cu = "INR";
            var req_upi = new Request_upiIntent
            {
                upiIntentURL = $"phonepe://pay?version=01&Mode=04&pa={pa}&pn={pn}&mc={mc}&tr={tr}&am={am}&tid={tid}&cu={cu}&tn={tn}"
            };


            var res_link = await StartAsyn_log(req_upi);


            if (res_link.Short_tinyLink.StartsWith("Error") || res_link.Short_tinyLink.StartsWith("Exception"))
            {
                return Content($"Error generating link: {res_link.Short_tinyLink}");
            }

            return Redirect(res_link.Short_tinyLink);
        }
        [HttpPost]
        public async Task<IActionResult> paytm(string loan_no, string transId, string amount)
        {
            if (string.IsNullOrEmpty(loan_no) || string.IsNullOrEmpty(transId) || string.IsNullOrEmpty(amount))
            {
                return BadRequest("Missing required parameters.");
            }
            string pa = "yogloans@idfcbank";
            string pn = loan_no;
            string tr = transId;
            string tn = "EMIPayment";

           string tid = "IDFCQPD1071931792705202474055401314";
            string am = amount;
            string mc = "7322";
            string cu = "INR";
            var req_upi = new Request_upiIntent
            {
                upiIntentURL = $"paytmmp://pay?Mode=04&pa={pa}&pn={pn}&mc={mc}&tr={tr}&am={am}&tid={tid}&cu={cu}&tn={tn}"
            };


            var res_link = await StartAsyn_log(req_upi);


            if (res_link.Short_tinyLink.StartsWith("Error") || res_link.Short_tinyLink.StartsWith("Exception"))
            {
                return Content($"Error generating link: {res_link.Short_tinyLink}");
            }

            return Redirect(res_link.Short_tinyLink);
        }


        public async Task<Response_shortlink> StartAsyn_log(Request_upiIntent req_upi)
        {
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // URL-encode the UPI Intent URL to make sure special characters are properly encoded
            string encodedUrl = Uri.EscapeDataString(req_upi.upiIntentURL);
            // TinyURL API endpoint
            string apiUrl = $"http://tinyurl.com/api-create.php?url={encodedUrl}";

            try
            {
                // Make the GET request to TinyURL API
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content (shortened URL)
                    string shortenedUrl = await response.Content.ReadAsStringAsync();

                    // Create and return the response with the shortened URL
                    return new Response_shortlink
                    {
                        Short_tinyLink = shortenedUrl
                    };
                }
                else
                {
                    // Handle failure if the response is not successful
                    return new Response_shortlink
                    {
                        Short_tinyLink = $"Error: {response.StatusCode}, {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                // Catch any exceptions and return the error message
                return new Response_shortlink
                {
                    Short_tinyLink = $"Exception: {ex.Message}"
                };
            }
        }
    }
}
