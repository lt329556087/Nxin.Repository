using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_VoucherAmortizationConfiguration : IEntityTypeConfiguration<FD_VoucherAmortization>
    {
        public void Configure(EntityTypeBuilder<FD_VoucherAmortization> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Number)
            .HasComment("摊销编码")
            .AddStringToLongConvert();

            entity.Property(e => e.AmortizationName)
            .HasComment("名称");

            entity.Property(e => e.TicketedPointID)
            .HasComment("单据字")
            .AddStringToLongConvert();

            entity.Property(e => e.AbstractID)
            .HasComment("摘要")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
             .HasComment("备注");

            entity.Property(e => e.OwnerID)
             .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.IsUse)
            .HasComment("是否启用");

            entity.Property(e => e.OperatorID)
             .HasComment("禁用人ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");
            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.FD_VoucherAmortization)
            .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();

            entity.HasMany(o => o.PeriodDetails)
           .WithOne(o => o.FD_VoucherAmortization)
           .HasForeignKey(o => o.NumericalOrder)
           .IsRequired();
        }
    }

    public class FD_VoucherAmortizationDetailConfiguration : IEntityTypeConfiguration<FD_VoucherAmortizationDetail>
    {
        public void Configure(EntityTypeBuilder<FD_VoucherAmortizationDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
          .HasComment("明细流水号")
          .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectCode)
            .HasComment("会计科目");

            entity.Property(e => e.AccoSubjectID)
            .HasComment("会计科目ID")
            .AddStringToLongConvert();

            entity.Property(e => e.PersonID)
            .HasComment("人员ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CustomerID)
            .HasComment("客户ID")
            .AddStringToLongConvert();

            entity.Property(e => e.MarketID)
            .HasComment("部门ID")
            .AddStringToLongConvert();

            entity.Property(e => e.SupplierID)
            .HasComment("供应商ID")
            .AddStringToLongConvert();

            entity.Property(e => e.ValueNumber)
            .HasComment("待摊销金额/比例");

            entity.Property(e => e.IsDebit)
            .HasComment("是否待摊销");
            
            entity.Property(e => e.ModifiedDate)
          .HasComment("最后修改日期");

        }
    }


    public class FD_VoucherAmortizationPeriodDetailConfiguration : IEntityTypeConfiguration<FD_VoucherAmortizationPeriodDetail>
    {
        public void Configure(EntityTypeBuilder<FD_VoucherAmortizationPeriodDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("记录标识");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
          .HasComment("明细流水号")
          .AddStringToLongConvert();

            entity.Property(e => e.RowNum)
            .HasComment("序号");

            entity.Property(e => e.AccountDate)
            .HasComment("会计期间");

            entity.Property(e => e.AmortizationAmount)
            .HasComment("摊销金额");

            entity.Property(e => e.IsAmort)
          .HasComment("是否摊销");

            entity.Property(e => e.IsLast)
          .HasComment("是否最后期间");

            entity.Property(e => e.ModifiedDate)
          .HasComment("最后修改日期");

        }
    }
}

