using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_UseStateTransferConfiguration : IEntityTypeConfiguration<FA_UseStateTransfer>
    {
        public void Configure(EntityTypeBuilder<FA_UseStateTransfer> entity)
        {

            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Number)
           .HasComment("单据号")
           .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
           .HasComment("日期");

            entity.Property(e => e.Remarks)
           .HasComment("备注");

            entity.Property(e => e.OwnerID)
            .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");

            entity.Property(e => e.ModifiedDate)
          .HasComment("修改日期");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.FA_UseStateTransfer)
             .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();
        }
    }

    public class FA_UseStateTransferDetailConfiguration : IEntityTypeConfiguration<FA_UseStateTransferDetail>
    {
        public void Configure(EntityTypeBuilder<FA_UseStateTransferDetail> entity)
        {

            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.CardID)
            .HasComment("卡片ID")
            .AddStringToLongConvert();

            entity.Property(e => e.BeforeUseStateID)
            .HasComment("变动前使用状态ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AfterUseStateID)
            .HasComment("变动后使用状态ID")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.ModifiedDate)
             .HasComment("修改日期");
        }
    }
}
