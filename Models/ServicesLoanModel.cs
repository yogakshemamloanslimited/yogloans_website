using System.Collections.Generic;
using yogloansdotnet.Models; // Make sure this using is present

namespace yogloansdotnet.Models
{

       public class ServicesLoanModel
{
    public List<HomwelcomeModel> Homwelcome { get; set; } = new List<HomwelcomeModel>();
    public List<AboutContentModel> AboutContent { get; set; } = new List<AboutContentModel>();
    public List<LoanPointModel> Loanpoint { get; set; } = new List<LoanPointModel>();
    public List<LoanModel> Loan { get; set; } = new List<LoanModel>();
     public List<OfferModel> Offer { get; set; } = new List<OfferModel>();
}

    }
