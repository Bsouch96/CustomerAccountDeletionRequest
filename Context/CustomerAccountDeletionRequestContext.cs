using CustomerAccountDeletionRequest.DomainModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Context
{
    public class CustomerAccountDeletionRequestContext : DbContext
    {
        public DbSet<DeletionRequestModel> DeletionRequestContext { get; set; }

        public CustomerAccountDeletionRequestContext(DbContextOptions<CustomerAccountDeletionRequestContext>
            options) : base(options)
        {

        }
    }
}
