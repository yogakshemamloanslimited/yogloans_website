using Microsoft.EntityFrameworkCore;
using yogloansdotnet.Migrations;
using yogloansdotnet.Models;

namespace yogloansdotnet.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AdminLoginModel> AdminLogins { get; set; }
        public DbSet<HomwelcomeModel> Homwelcome { get; set; }
        public DbSet<LoanModel> Loans { get; set; }
        public DbSet<LoanPointModel> LoanPoints { get; set; }
        public DbSet<AboutContentModel> AboutContent { get; set; }
        public DbSet<CountsModel> Counts { get; set; }

        public DbSet<OfferModel> Offer { get; set; }
        public DbSet<AnnualReportEntity> AnnualReports { get; set; }
        public DbSet<AnnouncementsModel> Announcements { get; set; }
        public DbSet<InvestoresGroup> Investor { get; set; }
        public DbSet<DisclosureModel> Disclosure { get; set; }
        public DbSet<FormdtsModel> Formdts { get; set; }
        public DbSet<UnpaidModel> Unpaid {get; set;}
        public DbSet<ApporderModel> Apporder {get; set;}
        public DbSet<CsrModel> Csr {get; set;}
        public DbSet<PolicyModel> Policy {get; set;}
        public DbSet<ServicesModel> Services {get; set;}
        public DbSet<CareerWelcomeModel> CareerWelcome{get; set;}
        public DbSet<yogloansdotnet.Models.AboutWelcome> AboutWelcome { get; set; }
        public DbSet<yogloansdotnet.Models.InvestorsWelcome> InvestorsWelcome { get; set; }
        public DbSet<yogloansdotnet.Models.OnlineWelcome> OnlineWelcome { get; set; }
        public DbSet<yogloansdotnet.Models.PolicyWelcome> PolicyWelcome { get; set; }
        public DbSet<yogloansdotnet.Models.CsrWelcome> CsrWelcome { get; set; }
        public DbSet<yogloansdotnet.Models.ContactWelcome> ContactWelcome { get; set; }
        public DbSet<yogloansdotnet.Models.DepartmentModel> Departments { get; set; }
        public DbSet<yogloansdotnet.Models.CareerModel> Career { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply seeders
            Seeders.AdminLoginSeeder.Seed(modelBuilder);
        }
    }
} 