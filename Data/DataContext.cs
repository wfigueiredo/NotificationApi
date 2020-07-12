using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Data
{
    public class DataContext : DbContext
    {
        private const string DEFAULT_SCHEMA = "notificationapi";

        public DbSet<Message> Messages { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MessageMapping());
            modelBuilder.HasDefaultSchema(DEFAULT_SCHEMA);
        }
    }
}
