using System.Collections.Generic;
using yogloansdotnet.Models; // Make sure this using is present

namespace yogloansdotnet.Models
{
    public class LoanGroupViewModel
    {
        public List<HomwelcomeModel> Gold { get; set; } = new List<HomwelcomeModel>();
        public List<HomwelcomeModel> Business { get; set; } = new List<HomwelcomeModel>();
        public List<HomwelcomeModel> Vehicle { get; set; } = new List<HomwelcomeModel>();
        public List<HomwelcomeModel> CD { get; set; } = new List<HomwelcomeModel>();
        public List<AboutContentModel> AboutContent { get; set; } = new List<AboutContentModel>();
        public List<LoanPointModel> Loanpoint { get; set; } = new List<LoanPointModel>(); // <-- Fix here
        public List<LoanModel> Loan { get; set; } = new List<LoanModel>(); // <-- And here
    }
}