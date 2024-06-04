using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_CapitalBudgetConfiguration : IEntityTypeConfiguration<FD_CapitalBudget>
    {

        public void Configure(EntityTypeBuilder<FD_CapitalBudget> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.OwnerID)
            .HasComment("制单人ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CapitalBudgetType)
            .HasComment("预算类型")
            .AddStringToLongConvert();

            entity.Property(e => e.TicketedPointID)
            .HasComment("票据")
            .AddStringToLongConvert();

            entity.Property(e => e.CapitalBudgetAbstract)
            .HasComment("预算类别")
            .AddStringToLongConvert();

            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.Guid)
            .HasComment("Guid");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");

            entity.Property(e => e.DataDate)
            .HasComment("单据日期");

            entity.Property(e => e.StartDate)
            .HasComment("开始时间");

            entity.Property(e => e.EndDate)
            .HasComment("结束时间");

            entity.Property(e => e.ModifiedDate)
            .HasComment("更新时间");

            entity.Property(e => e.MarketID)
           .HasComment("部门ID")
           .AddStringToLongConvert();

            entity.Property(e => e.Amount)
            .HasComment("部门ID");

        entity.HasMany(o => o.Details)
            .WithOne(o => o.FD_CapitalBudget)
            .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();
        }
    }

    public class FD_CapitalBudgetDetailConfiguration : IEntityTypeConfiguration<FD_CapitalBudgetDetail>
    {
        public void Configure(EntityTypeBuilder<FD_CapitalBudgetDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.PayAmount)
            .HasComment("支出金额");

            entity.Property(e => e.ReceiptAmount)
           .HasComment("收款金额");

            entity.Property(e => e.Guid)
            .HasComment("Guid");

            entity.Property(e => e.ReceiptAbstractID)
            .HasComment("摘要ID")
            .AddStringToLongConvert();

            entity.Property(e => e.PaymentObjectID)
            .HasComment("付款对象")
            .AddStringToLongConvert();
        }
    }

}
