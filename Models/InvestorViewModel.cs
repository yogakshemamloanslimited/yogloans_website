using System.Collections.Generic;

namespace yogloansdotnet.Models
{
    public class InvestorViewModel
    {
        public IEnumerable<AnnualReportEntity> Report { get; set; } = new List<AnnualReportEntity>();
        public IEnumerable<AnnouncementsModel> Announcements { get; set; } = new List<AnnouncementsModel>();
        public IEnumerable<InvestoresGroup> Investor { get; set; } = new List<InvestoresGroup>();
        public IEnumerable<DisclosureModel> Disclosure { get; set; } = new List<DisclosureModel>();
        public IEnumerable<FormdtsModel> Formdts {get; set;} = new List <FormdtsModel>();
         public IEnumerable<UnpaidModel> Unpaid {get; set;} = new List <UnpaidModel>();
        public IEnumerable<ApporderModel> Apporder {get; set;} = new List <ApporderModel>();
    }
}
