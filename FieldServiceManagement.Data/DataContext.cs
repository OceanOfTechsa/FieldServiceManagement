using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Announcement;
using FieldServiceManagement.Data.DataModels.Asset;
using FieldServiceManagement.Data.DataModels.Audits;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.Data.DataModels.Country;
using FieldServiceManagement.Data.DataModels.Currency;
using FieldServiceManagement.Data.DataModels.Customer;
using FieldServiceManagement.Data.DataModels.Department;
using FieldServiceManagement.Data.DataModels.Disclaimer;
using FieldServiceManagement.Data.DataModels.EmailErrorLog;
using FieldServiceManagement.Data.DataModels.EmailLog;
using FieldServiceManagement.Data.DataModels.EmailQueue;
using FieldServiceManagement.Data.DataModels.Form;
using FieldServiceManagement.Data.DataModels.FormApprovalProgress;
using FieldServiceManagement.Data.DataModels.FormSubmission;
using FieldServiceManagement.Data.DataModels.FormSubmissionAudit;
using FieldServiceManagement.Data.DataModels.FormSubmissionStageDecision;
using FieldServiceManagement.Data.DataModels.FormSubmissionStageState;
using FieldServiceManagement.Data.DataModels.Industry;
using FieldServiceManagement.Data.DataModels.Invoice;
using FieldServiceManagement.Data.DataModels.Language;
using FieldServiceManagement.Data.DataModels.Notification;
using FieldServiceManagement.Data.DataModels.Organisation;
using FieldServiceManagement.Data.DataModels.OrganisationSubscription;
using FieldServiceManagement.Data.DataModels.Payment;
using FieldServiceManagement.Data.DataModels.ServiceAppointment;
using FieldServiceManagement.Data.DataModels.ServiceReport;
using FieldServiceManagement.Data.DataModels.Settings;
using FieldServiceManagement.Data.DataModels.State;
using FieldServiceManagement.Data.DataModels.Status;
using FieldServiceManagement.Data.DataModels.SubmissionDocument;
using FieldServiceManagement.Data.DataModels.SubscriptionPlan;
using FieldServiceManagement.Data.DataModels.TripLog;
using FieldServiceManagement.Data.DataModels.UploadDocument;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Data.DataModels.UserProfileAudit;
using FieldServiceManagement.Data.DataModels.UserRole;
using FieldServiceManagement.Data.DataModels.WorkflowApproverAssignment;
using FieldServiceManagement.Data.DataModels.WorkflowEscalationRule;
using FieldServiceManagement.Data.DataModels.WorkflowStageDefinition;
using FieldServiceManagement.Data.DataModels.WorkOrder;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using FieldServiceManagement.ViewModels.Settings;
using FieldServiceManagement.Data.DataModels.Crew;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FieldServiceManagement.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IDbContext
    {
        #region Reference Tables
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<DataModels.Timezone.Timezone> Timezones { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<IndustryCategory> IndustryCategories { get; set; }
        public DbSet<AppUserRole> AppUserRoles { get; set; }       // renamed — avoids Identity conflict
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Disclaimer> Disclaimers { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<UploadDocument> UploadDocuments { get; set; }
        public DbSet<AnnouncementDeliveryType> AnnouncementDeliveryTypes { get; set; }
        #endregion

        #region Workflow
        public DbSet<WorkflowStageDefinition> WorkflowStageDefinitions { get; set; }
        public DbSet<WorkflowApproverAssignment> WorkflowApproverAssignments { get; set; }
        public DbSet<WorkflowEscalationRule> WorkflowEscalationRules { get; set; }
        #endregion

        #region Organisation
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationSubscription> OrganisationSubscriptions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Department> Departments { get; set; }
        #endregion

        #region Users
        public DbSet<AppUser> AppUsers { get; set; }               
        public DbSet<UserProfileAudit> UserProfileAudits { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        #endregion

        #region Forms
        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<FormSubmissionStageState> FormSubmissionStageStates { get; set; }
        public DbSet<FormApprovalProgress> FormApprovalProgresses { get; set; }
        public DbSet<FormSubmissionStageDecision> FormSubmissionStageDecisions { get; set; }
        public DbSet<FormSubmissionAudit> FormSubmissionAudits { get; set; }
        public DbSet<SubmissionDocument> SubmissionDocuments { get; set; }
        #endregion

        #region Operations
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<ServiceAppointment> ServiceAppointments { get; set; }
        public DbSet<TripLog> TripLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ServiceReport> ServiceReports { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Crew> Crews { get; set; }
        public DbSet<CrewMember> CrewMembers { get; set; }
        #endregion

        #region Error
        public DbSet<EmailErrorLog> EmailErrorLogs { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }
        #endregion

        #region UserInvitation
        public DbSet<UserInvitation> UserInvitations { get; set; }
        #endregion

        #region System
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Settings> Settings { get; set; }
        #endregion

        public DataContext() { }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public new EntityEntry Entry(object entity)
        {
            return base.Entry(entity);
        }

        public static DataContext Create()
        {
            return new DataContext();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new SqlConnection(ServicesExtensions.FSMConnectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Circular FK — Organisation ↔ User
            modelBuilder.Entity<Organisation>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<AppUser>()
                .HasOne<Organisation>()
                .WithMany()
                .HasForeignKey(u => u.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<AppUserProfile>().HasNoKey().ToView(null);
            modelBuilder.Entity<UserAnnouncement>().HasNoKey();
            modelBuilder.Entity<AnnouncementAdminListItem>().HasNoKey();
            modelBuilder.Entity<SettingsDetailsForDisplay>().HasNoKey();
            modelBuilder.Entity<UserNotification>().HasNoKey();
        }
    }
}