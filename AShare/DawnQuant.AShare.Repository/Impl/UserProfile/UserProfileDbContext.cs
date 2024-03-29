﻿using DawnQuant.Repository;
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

        //自选
        public DbSet<SelfSelectStockCategory> SelfSelectStockCategories { get; set; }
        public DbSet<SelfSelectStock> SelfSelectStocks { get; set; }


        //龙头
        public DbSet<BellwetherStockCategory> BellwetherStockCategories { get; set; }
        public DbSet<BellwetherStock> BellwetherStocks { get; set; }

        //题材热点
        public DbSet<SubjectAndHotStockCategory> SubjectAndHotStockCategories { get; set; }
        public DbSet<SubjectAndHotStock> SubjectAndHotStocks { get; set; }

        //策略
        public DbSet<StockStrategy>  StockStrategies { get; set; }
        public DbSet<StockStrategyCategory>  StockStrategyCategories { get; set; }


        //计划任务
        public DbSet<StrategyScheduledTask>  StrategyScheduledTasks { get; set; }


        //配置信息
        public DbSet<Setting> Settings { get; set; }


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
