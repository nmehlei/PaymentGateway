using System;
using Microsoft.EntityFrameworkCore.Design;

namespace PaymentGateway.Persistence.Helpers
{
    public class DataContextDesignTimeFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            // NOTE: This allows to use an SQL DB outside of the Docker environment for fast prototyping.
            var connectionString = "Server=localhost,14330;Database=PaymentGatewayDesignDB;User Id=sa;Password=change_this_password;MultipleActiveResultSets=true";

            return new DataContext(connectionString);
        }
    }
}
