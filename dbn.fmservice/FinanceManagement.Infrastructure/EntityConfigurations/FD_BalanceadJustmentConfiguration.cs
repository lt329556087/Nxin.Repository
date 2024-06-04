using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_BalanceadJustmentConfiguration : IEntityTypeConfiguration<FD_BalanceadJustment>
    {
        public void Configure(EntityTypeBuilder<FD_BalanceadJustment> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();
            
            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();


            entity.Property(e => e.DataDate)
            .HasComment("日期");

            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.AccountID)
            .HasComment("资金账户ID")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.OwnerID)
            .HasComment("制单人ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");
            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.FD_BalanceadJustment)
            .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();


        }
    }

    public class FD_BalanceadJustmentDetailConfiguration : IEntityTypeConfiguration<FD_BalanceadJustmentDetail>
    {
        public void Configure(EntityTypeBuilder<FD_BalanceadJustmentDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterProjectID)
            .HasComment("企业项目ID")
            .AddStringToLongConvert();
            entity.Property(e => e.EnterProjectAmount)
            .HasComment("企业项目金额");

            entity.Property(e => e.BankProjectID)
            .HasComment("银行项目ID")
            .AddStringToLongConvert();
            entity.Property(e => e.BankProjectAmount)
            .HasComment("银行项目金额");

        }
    }
}

