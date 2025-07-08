using System.Collections.Generic;

namespace yogloansdotnet.Models

{
    public class InvestoreViewModel
    {
        public List<InvestorsWelcome> InvestorsWelcome { get; set; } = new();
        public List<AboutContentModel> AboutContent { get; set; } = new();
        public List<DirectorsModel> Directors { get; set; } = new();
        public List<LeadersModel> Leaders { get; set; } = new();
    }
}
