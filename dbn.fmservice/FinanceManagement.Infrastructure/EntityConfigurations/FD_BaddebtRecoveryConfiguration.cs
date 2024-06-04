using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_BaddebtRecoveryConfiguration : IEntityTypeConfiguration<FD_BaddebtRecovery>
    {
        public void Configure(EntityTypeBuilder<FD_BaddebtRecovery> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                .HasName("PRIMARY");

            entity.HasComment("坏账收回");

            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();
            

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasComment("创建日期");

            entity.Property(e => e.DataDate)
                .HasColumnType("date")
                .HasComment("日期");

            entity.Property(e => e.EnterpriseID)
                .HasColumnType("bigint(20)")
                .HasComment("单位ID")
               .AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后修改日期")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.OwnerID)
                .HasColumnType("bigint(20)")
                .HasComment("制单人")
                .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
                .HasColumnType("varchar(128)")
                .HasComment("备注");

            entity.Property(e => e.TicketedPointID)
                .HasColumnType("bigint(20)")
                .HasComment("单据字")
                 .AddStringToLongConvert();

            entity.Property(e => e.Number)
               .HasColumnType("bigint(20)")
               .HasComment("单据号")
                .AddStringToLongConvert();

            entity.HasMany(o => o.Lines)
               .WithOne(o => o.FD_BaddebtRecovery)
               .HasForeignKey(o => o.NumericalOrder)
               .IsRequired();
        }      
    }

    public class FD_BaddebtRecoveryDetailConfiguration : IEntityTypeConfiguration<FD_BaddebtRecoveryDetail>
    {
        public void Configure(EntityTypeBuilder<FD_BaddebtRecoveryDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .HasComment("期末金额");

            entity.Property(e => e.BusiType)
                .HasColumnType("bigint(20)")
                .HasComment("业务类型（0：应收账款 1：其他应收款）");

            entity.Property(e => e.BusinessType)
                .HasColumnType("bigint(20)")
                .HasComment("往来类型")
                .AddStringToLongConvert();

            entity.Property(e => e.CurrentUnit)
                .HasColumnType("bigint(20)")
                .HasComment("往来单位")
                .AddStringToLongConvert();
          

            entity.Property(e => e.RecoveryAmount)
               .HasColumnType("decimal(18,2)")
               .HasComment("坏账收回金额");

            entity.Property(e => e.ModifiedDate)
           .HasComment("最后修改日期");
        }
    }

}
