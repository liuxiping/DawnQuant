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

  
        //股东人数
        public DbSet<HolderNumber> HolderNumbers { get; set; }

        //十大流通股
        public DbSet<Top10FloatHolder> Top10FloatHolders { get; set; }

        /// <summary>
        /// 龙头股
        /// </summary>
        public DbSet<Bellwether> Bellwethers { get; set; }


        /// <summary>
        /// 题材热点
        /// </summary>
        public DbSet<SubjectAndHot> SubjectAndHots { get; set;}

        /// <summary>
        /// 业绩预测
        /// </summary>
        public DbSet<PerformanceForecast> PerformanceForecasts { get; set; }


        /// <summary>
        /// 同花顺指数
        /// </summary>
        public DbSet<THSIndex> THSIndexes { get; set; }

        /// <summary>
        /// 同花顺指数成员
        /// </summary>
        public DbSet<THSIndexMember> THSIndexMembers { get; set; }
        /// <summary>
        /// 题材前瞻
        /// </summary>
        public DbSet<FutureEventOfSubject> FutureEventsOfSubject { get; set; }

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


            modelBuilder.Entity<HolderNumber>(en =>
            {
                en.HasIndex(p =>new { p.TSCode, p.EndDate }).IsUnique();

            });

            modelBuilder.Entity<Top10FloatHolder>(en =>
            {
                en.HasIndex(p => new { p.TSCode, p.EndDate ,p.HolderName}).IsUnique();

            });

            modelBuilder.Entity<Bellwether>(en =>
            {
                en.HasIndex(p => new { p.TSCode, p.Source }).IsUnique();

            });

            modelBuilder.Entity<SubjectAndHot>(en =>
            {
                en.HasIndex(p => new { p.TSCode, p.Source }).IsUnique();

            });

            modelBuilder.Entity<PerformanceForecast>(en =>
            {
                en.HasIndex(p => new { p.TSCode, p.EndDate, p.Source }).IsUnique();

            });

            modelBuilder.Entity<FutureEventOfSubject>(en =>
            {
                en.HasIndex(p => new { p.Date, p.Event }).IsUnique();

            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
           
        }
    }
}
