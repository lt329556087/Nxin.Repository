using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;


namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_BadDebtOccurConfiguration : IEntityTypeConfiguration<FD_BadDebtOccur>
    {
        public void Configure(EntityTypeBuilder<FD_BadDebtOccur> entity)
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
            .HasComment("坏账科目一")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID1)
            .HasComment("坏账科目一")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID2)
            .HasComment("坏账科目二")
            .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
             .HasComment("单据日期");

            entity.Property(e => e.CreateDate)
            .HasComment("单据日期");

            //entity.HasMany(o => o.Lines)
            //.WithOne(o => o.FD_BadDebtOccur)
            // .HasForeignKey(o => o.NumericalOrder)
            //.IsRequired();

            entity.Property(e => e.PersonID)
            .HasComment("员工ID")
            .AddStringToLongConvert();

            entity.Property(e => e.BusinessType)
            .HasComment("往来类型")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderSetting)
            .HasComment("流水号")
            .AddStringToLongConvert();
        }
    }


    public class FD_BadDebtOccurDetailConfiguration : IEntityTypeConfiguration<FD_BadDebtOccurDetail>
    {
        public void Configure(EntityTypeBuilder<FD_BadDebtOccurDetail> entity)
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

            entity.Property(e => e.CurrentOccurAmount)
            .HasComment("未收回金额");

            entity.Property(e => e.Amount)
             .HasComment("本期坏账计提准备金额");


   
        }
    }
}
