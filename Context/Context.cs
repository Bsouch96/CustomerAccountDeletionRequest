using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Context
{
    public class Context : DbContext
    {
        public DbSet<DeletionRequestModel> _deletionRequestContext { get; set; }
        private readonly IOptionsMonitor<DatabaseAttributesModel> _optionsMonitor;

        public Context(DbContextOptions<Context> options, IOptionsMonitor<DatabaseAttributesModel> optionsMonitor) : base(options)
        {
            _optionsMonitor = optionsMonitor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeletionRequestModel>().ToTable(_optionsMonitor.CurrentValue.TableName, "Staging")
                .HasData(
                    new DeletionRequestModel { DeletionRequestID = 1, CustomerID = 1, DeletionReason = "Staging Terrible Site.", DateRequested = new System.DateTime(2010, 10, 01, 8, 5, 3), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                    new DeletionRequestModel { DeletionRequestID = 2, CustomerID = 2, DeletionReason = "Staging Prefer Amazon.", DateRequested = new System.DateTime(2012, 01, 02, 10, 3, 45), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                    new DeletionRequestModel { DeletionRequestID = 3, CustomerID = 3, DeletionReason = "Staging Too many clicks.", DateRequested = new System.DateTime(2013, 02, 03, 12, 2, 40), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                    new DeletionRequestModel { DeletionRequestID = 4, CustomerID = 4, DeletionReason = "Staging Scammed into signing up.", DateRequested = new System.DateTime(2014, 03, 04, 14, 1, 35), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 3, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                    new DeletionRequestModel { DeletionRequestID = 5, CustomerID = 5, DeletionReason = "Staging If Wish was built by students...", DateRequested = new System.DateTime(2007, 04, 05, 16, 50, 30), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 4, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision }
                );
            
            modelBuilder.Entity<DeletionRequestModel>().ToTable(_optionsMonitor.CurrentValue.TableName, "Production")
                .HasData(
                    new DeletionRequestModel { DeletionRequestID = 6, CustomerID = 1, DeletionReason = "Production Terrible Site.", DateRequested = new System.DateTime(2010, 10, 01, 8, 5, 3), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                    new DeletionRequestModel { DeletionRequestID = 7, CustomerID = 2, DeletionReason = "Production Prefer Amazon.", DateRequested = new System.DateTime(2012, 01, 02, 10, 3, 45), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                    new DeletionRequestModel { DeletionRequestID = 8, CustomerID = 3, DeletionReason = "Production Too many clicks.", DateRequested = new System.DateTime(2013, 02, 03, 12, 2, 40), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                    new DeletionRequestModel { DeletionRequestID = 9, CustomerID = 4, DeletionReason = "Production Scammed into signing up.", DateRequested = new System.DateTime(2014, 03, 04, 14, 1, 35), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 3, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                    new DeletionRequestModel { DeletionRequestID = 10, CustomerID = 5, DeletionReason = "Production If Wish was built by students...", DateRequested = new System.DateTime(2007, 04, 05, 16, 50, 30), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 4, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision }
                );

            modelBuilder.Entity<DeletionRequestModel>().ToTable(_optionsMonitor.CurrentValue.TableName, _optionsMonitor.CurrentValue.Schema);
        }
    }
}
