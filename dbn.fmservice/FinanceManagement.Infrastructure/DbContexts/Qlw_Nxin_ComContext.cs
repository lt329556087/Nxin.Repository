// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Architecture.Seedwork.Infrastructure;
using FinanceManagement.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FinanceManagement.Infrastructure
{
    public partial class Qlw_Nxin_ComContext : EFContext
    {


        public Qlw_Nxin_ComContext(DbContextOptions<Qlw_Nxin_ComContext> options, IMediator mediator)
            : base(options, mediator)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region 基础
            //modelBuilder.ApplyConfiguration(new BizEnterpriseEntityConfigurations());
            //modelBuilder.ApplyConfiguration(new BizMarketEntityConfigurations());

            modelBuilder.ApplyConfiguration(new FD_CapitalBudgetConfiguration());

            modelBuilder.ApplyConfiguration(new FD_CapitalBudgetDetailConfiguration());

            modelBuilder.ApplyConfiguration(new FD_ExpenseConfiguration());

            modelBuilder.ApplyConfiguration(new FD_ExpenseDetailConfiguration());
            modelBuilder.ApplyConfiguration(new FD_ExpenseExtConfiguration());
            modelBuilder.ApplyConfiguration(new bsfileConfiguration());

            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
