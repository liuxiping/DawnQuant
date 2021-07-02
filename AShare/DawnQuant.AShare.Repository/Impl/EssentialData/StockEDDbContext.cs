using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.Repository;
using DawnQuant.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace DawnQuant.AShare.Repository.Impl.EssentialData
{
    /// <summary>
    /// 股票数据数据库存储
    /// </summary>
    public class StockEDDbContext : DbContext
    {

        public StockEDDbContext(DbContextOptions<StockEDDbContext> options) : base(options)
        {

        }

        public DbSet<Industry> Industrys { get; set; }

        public DbSet<Company> Companys { get; set; }

        public DbSet<TradingCalendar> TradingCalendars { get; set; }

        public DbSet<BasicStockInfo > BasicStockInfos { get; set; }

        public DbSet<StockFormerName> StockFormerNames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Industry>(en =>
            {

                en.HasIndex(this.GetPropertyName<Industry,string>(p=>p.Name),
                    this.GetPropertyName<Industry, int>(p=>p.Level)).IsUnique();
                
                
            });
            modelBuilder.Entity<Company>(en =>
            {
               

            });

            modelBuilder.Entity<TradingCalendar>(en =>
            {
                //交易所和日期
                en.HasIndex(p => new { p.Date, p.Exchange }).IsUnique();

            });

            modelBuilder.Entity<BasicStockInfo>(en =>
            {
                en.Property(p => p.StockCode).IsUnicode();

            });



        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
           
        }
    }
}
