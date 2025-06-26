using System.Collections.Generic;

namespace yogloansdotnet.Models
{
    public class LoanGroupViewModel
    {
        public List<HomwelcomeModel> Gold { get; set; } = new List<HomwelcomeModel>();
        public List<HomwelcomeModel> Business { get; set; } = new List<HomwelcomeModel>();
        public List<HomwelcomeModel> Vehicle { get; set; } = new List<HomwelcomeModel>();
        public List<HomwelcomeModel> CD { get; set; } = new List<HomwelcomeModel>();
    }
}
