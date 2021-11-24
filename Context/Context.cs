using CustomerAccountDeletionRequest.DomainModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Context
{
    public class Context : DbContext
    {
        public DbSet<DeletionRequestModel> _deletionRequestContext { get; set; }

        public Context(DbContextOptions<Context>
            options) : base(options)
        {

        }
    }
}
