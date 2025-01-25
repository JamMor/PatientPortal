using Microsoft.EntityFrameworkCore;

namespace PatientPortal.Models
{
    public class PatientPortalContext : DbContext
    {
        public PatientPortalContext(DbContextOptions options) : base(options) { }

        public DbSet<Staff> Staff { get;set; }
        public DbSet<Patient> Patients { get;set; }
        public DbSet<PatientStaffConnection> PatientStaffConnections { get;set; }
        public DbSet<Address> Addresses { get;set; }
        public DbSet<HealthIssue> HealthIssues { get;set; }
        public DbSet<Visit> Visits { get;set; }
        public DbSet<TestResult> TestResults { get;set; }
        public DbSet<TestHealthIssueAssociation> TestHealthIssueAssociations { get;set; }
        public DbSet<VisitHealthIssueAssociation> VisitHealthIssueAssociations { get;set; }

        //Messaging
        public DbSet<MessagingLink> MessagingLinks { get;set; }
        public DbSet<Conversation> Conversations { get;set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get;set; }
        public DbSet<Message> Messages { get;set; }
        public DbSet<Unread> UnreadMessages { get;set; }
    }
}