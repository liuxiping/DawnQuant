using DawnQuant.Repository;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DawnQuant.AShare.Entities;

namespace DawnQuant.AShare.Repository.Impl.UserProfile
{
    /// <summary>
    /// 股票数据数据库存储
    /// </summary>
    public class UserProfileDbContext : DbContext
    {

        public UserProfileDbContext(DbContextOptions<UserProfileDbContext> options) : base(options)
        {

        }

        public DbSet<SelfSelectStockCategory> SelfSelectStockCategorys { get; set; }

        public DbSet<SelfSelectStock> SelfSelectStocks { get; set; }

        public DbSet<StockStrategy>  StockStrategies { get; set; }

        public DbSet<StockStrategyCategory>  StockStrategyCategories { get; set; }

        public DbSet<StrategyScheduledTask>  StrategyScheduledTasks { get; set; }

        public DbSet<ExclusionStock>  ExclusionStocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SelfSelectStockCategory>(en =>
            {
                en.HasIndex(p => p.Id);
                en.HasIndex(p => p.UserId);
            });
            modelBuilder.Entity<SelfSelectStock>(en =>
            {
                en.HasIndex(p => p.Id);
                en.HasIndex(p => p.UserId);
                en.HasIndex(p => p.CategoryId);
            });

          
            modelBuilder.Entity<StockStrategy>(en =>
            {
                en.HasIndex(p => p.Id);
                en.HasIndex(p => p.UserId);
                en.HasIndex(p => p.CategoryId);
            });
            modelBuilder.Entity<StockStrategyCategory>(en =>
            {
                en.HasIndex(p => p.Id);
                en.HasIndex(p => p.UserId);
            });

            modelBuilder.Entity<StrategyScheduledTask>(en =>
            {
                en.HasIndex(p => p.Id);
                en.HasIndex(p => p.UserId);
            });

            modelBuilder.Entity<ExclusionStock>(en =>
            {
                en.HasIndex(p => p.Id);
                en.HasIndex(p => p.UserId);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
           
        }
    }
}
