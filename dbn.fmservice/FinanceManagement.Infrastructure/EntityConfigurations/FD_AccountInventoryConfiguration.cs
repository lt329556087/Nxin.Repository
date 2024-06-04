using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_AccountInventoryConfiguration : IEntityTypeConfiguration<FD_AccountInventory>
    {
        public void Configure(EntityTypeBuilder<FD_AccountInventory> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Guid)
            .HasComment("全球唯一值");

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();


            entity.Property(e => e.DataDate)
            .HasComment("单据日期");


            entity.Property(e => e.TicketedPointID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.TicketedPointID)
            .HasComment("字号")
            .AddStringToLongConvert();


            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.ResponsiblePerson)
            .HasComment("负责人ID")
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
            .WithOne(o => o.FD_AccountInventory)
            .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();


        }
    }

    public class FD_AccountInventoryDetailConfiguration : IEntityTypeConfiguration<FD_AccountInventoryDetail>
    {
        public void Configure(EntityTypeBuilder<FD_AccountInventoryDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自动增长ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Guid)
            .HasComment("全球唯一关键字");

            entity.Property(e => e.AccountID)
            .HasComment("账号ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID)
            .HasComment("科目ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectCode)
            .HasComment("科目编码");

            entity.Property(e => e.FlowAmount)
            .HasComment("流动资金");

            entity.Property(e => e.DepositAmount)
            .HasComment("定期存款");

            entity.Property(e => e.FrozeAmount)
            .HasComment("不可用资金");

            entity.Property(e => e.FuturesBond)
            .HasComment("期货保证金");

            entity.Property(e => e.OtherBond)
            .HasComment("其他保证金");

            entity.Property(e => e.BankFrozen)
            .HasComment("银行冻结");

            entity.Property(e => e.OtherAmount)
            .HasComment("其他");

            entity.Property(e => e.BookAmount)
            .HasComment("账面金额");

            entity.Property(e => e.Remarks)
            .HasComment("备注");
        }
    }
}

