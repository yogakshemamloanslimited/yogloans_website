using System.Collections.Generic;

namespace yogloansdotnet.Models
{
    public class LoanPointsViewModel
    {
        public IEnumerable<LoanModel> Loans { get; set; } = new List<LoanModel>();
        public IEnumerable<LoanPointModel> Points { get; set; } = new List<LoanPointModel>();
        public IEnumerable<OfferModel> Offer { get; set; } = new List<OfferModel>();
    }
} 