using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_BadDebtRecoverConfiguration : IEntityTypeConfiguration<FD_BadDebtRecover>
    {
        public void Configure(EntityTypeBuilder<FD_BadDebtRecover> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
           .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.TicketedPointID)
            .HasComment("核算单元ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CustomerID)
            .HasComment("客户ID")
            .AddStringToLongConvert();


            entity.Property(e => e.CAccoSubjectID)
             .HasComment("贷方科目ID")
             .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID1)
            .HasComment("客户ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID2)
             .HasComment("客户ID")
             .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
            .HasComment("单据日期");

            entity.Property(e => e.CreateDate)
            .HasComment("单据日期");

            entity.Property(e => e.NumericalOrderSetting)
             .HasComment("流水号")
             .AddStringToLongConvert();

            entity.Property(e => e.PersonID)
             .HasComment("员工ID")
             .AddStringToLongConvert();

            entity.Property(e => e.BusinessType)
             .HasComment("往来类型")
             .AddStringToLongConvert();
        }
    }

    public class FD_BadDebtRecoverDetailConfiguration : IEntityTypeConfiguration<FD_BadDebtRecoverDetail>
    {
        public void Configure(EntityTypeBuilder<FD_BadDebtRecoverDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
              .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.PersonID)
            .HasComment("员工ID")
            .AddStringToLongConvert();

            entity.Property(e => e.MarketID)
            .HasComment("部门ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID)
           .HasComment("科目ID")
           .AddStringToLongConvert();

            entity.Property(e => e.CurrentRecoverAmount)
            .HasComment("本期收回金额");

            entity.Property(e => e.Amount)
             .HasComment("余额");
        }
    }
}
