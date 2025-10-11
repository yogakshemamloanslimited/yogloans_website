using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;
using yogloansdotnet.Migrations;

namespace yogloansdotnet.Controllers
{
    [Route("[controller]")]
    public class GenerateqrcodeController : Controller
    {
        private readonly ILogger<GenerateqrcodeController> _logger;
        private static readonly List<LoanPayment> _payments = new List<LoanPayment>();


        public GenerateqrcodeController(ILogger<GenerateqrcodeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("~/Views/Generateqrcode/index.cshtml");
        }
        public class LoanPayment
        {
            public int Id { get; set; }
            public string LoanNo { get; set; }
            public string CustomerName { get; set; }
            public string TransactionId { get; set; }
            public decimal Amount { get; set; }
            public string UPIReferenceId { get; set; }
            public string Status { get; set; } = "PENDING"; // PENDING, SUCCESS, FAILED
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public DateTime? PaidAt { get; set; }
        }
       

        [HttpGet]
        [Route("GenerateQRCode")]
        public IActionResult GenerateQRCode(string loanno, string customername)
        {
            loanno = string.IsNullOrEmpty(loanno) ? "105297" : loanno;
            customername = string.IsNullOrEmpty(customername) ? "Anas" : customername;

            var transactionId = $"TXN{loanno}{DateTime.Now.Ticks}";

            var payment = new LoanPayment
            {
                LoanNo = loanno,
                CustomerName = customername,
                TransactionId = transactionId,
                Status = "PENDING",
           
            };
            _payments.Add(payment);

            try
            {
                var qrData = $"upi://pay?ver=01&Mode=04&orgId=700001&tr={transactionId}&tn=Loan%20No%20-%20{loanno}&pa=yog.{loanno}@idfcbank&pn=Yog%20Loans&mc=6011";


                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new QRCode(qrCodeData))
                    using (var bitmap = qrCode.GetGraphic(20))
                    using (var stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Png);
                        var base64 = Convert.ToBase64String(stream.ToArray());
                        
                        return Ok(new
                        {
                            success = true,
                            transactionId,
                            qrImage = $"data:image/png;base64,{base64}",
                            qrData
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate QR for loan {LoanNo}", loanno);
                // Return JSON body and 500 status so client can handle it gracefully
                return StatusCode(500, new { success = false, message = "Server error generating QR: " + ex.Message });
            }
        }


        //[HttpGet]
        //[Route("GetPaymentByTransactionId")]
        //public IActionResult GetPaymentByTransactionId(string transactionId)
        //{
        //    // Find the payment where the TransactionId matches the input string
        //    var transitionok = _payments.FirstOrDefault(p => p.TransactionId == transactionId);

        //    if (transitionok != null && transitionok.TransactionId == transactionId)
        //    {
        //        // Return JSON with status
        //        return Json(new
        //        {
        //            success = true,
        //            status = transitionok.Status
        //        });
        //    }
        //    else
        //    {
        //        // TransactionId not found or doesn't match
        //        return Json(new
        //        {
        //            success = false,
        //            message = "Transaction not found"
        //        });
        //    }
        //}


        //[HttpGet]
        //[Route("CheckPaymentStatus")]
        //public IActionResult CheckPaymentStatus(string transactionId)
        //{
        //    var payment = _payments.FirstOrDefault(p => p.TransactionId == transactionId);
        //    if (payment == null)
        //        return Json(new { success = false, message = "Transaction not found" });

        //    return Json(new { success = true, status = payment.Status });
        //}

        //[HttpPost]
        //[Route("PaymentCallback")]
        //public IActionResult PaymentCallback([FromBody] PaymentResponseModel response)
        //{
        //    if (response == null || string.IsNullOrEmpty(response.TransactionId))
        //        return BadRequest("Invalid payment response");

        //    var payment = _payments.FirstOrDefault(p => p.TransactionId == response.TransactionId);

        //    if (payment == null)
        //        return NotFound("Transaction not found");

        //    if (response.Status == "SUCCESS")
        //    {
        //        payment.Status = "SUCCESS";
        //        payment.PaidAt = DateTime.Now;
        //        payment.UPIReferenceId = response.UPIReferenceId;
        //        return Json(new { success = true, message = "Payment confirmed" });
        //    }

        //    payment.Status = "FAILED";
        //    return Json(new { success = false, message = "Payment failed or pending" });


        //public class PaymentResponseModel
        //{
        //    public string TransactionId { get; set; }
        //    public string Status { get; set; } // SUCCESS, FAILED
        //    public decimal Amount { get; set; }
        //    public string UPIReferenceId { get; set; }
        //    public DateTime PaidAt { get; set; }
        //}
        //[HttpGet]
        //[Route("SimulateScan")]
        //public IActionResult SimulateScan(string transactionId)
        //{
        //    var payment = _payments.FirstOrDefault(p => p.TransactionId == transactionId);
        //    if (payment == null)
        //        return Json(new { success = false, message = "Transaction not found" });

        //    // Mark as scanned (still pending payment)
        //    payment.Status = "SCANNED";

        //    return Json(new { success = true, message = "QR Scanned" });
        //}



    }
}

