using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_CashSweepSettingConfiguration : IEntityTypeConfiguration<FM_CashSweepSetting>
    {
        public void Configure(EntityTypeBuilder<FM_CashSweepSetting> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                .HasName("PRIMARY");
            

            entity.Property(e => e.NumericalOrder)
                .HasComment("流水号")
                .AddStringToLongConvert(); 

            entity.Property(e => e.CreatedDate)
                .HasComment("创建日期");

            entity.Property(e => e.EnterpriseID)
                .HasComment("集团ID")
                .AddStringToLongConvert(); 

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.OwnerID)
                .HasComment("制单人ID")
                 .AddStringToLongConvert(); 

            entity.Property(e => e.Remarks)
                .HasComment("备注");

            entity.HasIndex(e => e.EnterpriseID)
                .HasName("index_EnterpriseID");

            entity.HasMany(o => o.Lines)
           .WithOne(o => o.FM_CashSweepSetting)
            .HasForeignKey(o => o.NumericalOrder)
           .IsRequired();
        }
    }

    public class FM_CashSweepSettingDetailConfiguration : IEntityTypeConfiguration<FM_CashSweepSettingDetail>
    {
        public void Configure(EntityTypeBuilder<FM_CashSweepSettingDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

            entity.HasIndex(e => e.NumericalOrder)
                .HasName("index_NumericalOrder");

            entity.Property(e => e.RecordID)
                .HasComment("主键");

            entity.Property(e => e.AccountTransferAbstract)
                .HasComment("调拨类别")
                 .AddStringToLongConvert();                      

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间");

            entity.Property(e => e.NumericalOrder)
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
                .HasComment("详情流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
                .HasComment("调拨事由");

            entity.Property(e => e.OwnerID)
               .HasComment("制单人")
               .AddStringToLongConvert();

            entity.Property(e => e.SweepDirection)
                .HasComment("归集方向(0：向上归集；1：向下归集)");

          //  entity.HasMany(o => o.Extends)
          //.WithOne(o => o.FM_CashSweepSettingDetail)
          // .HasForeignKey(o => o.NumericalOrderDetail)
          //.IsRequired();
        }
    }


    public class FM_CashSweepSettingExtConfiguration : IEntityTypeConfiguration<FM_CashSweepSettingExt>
    {
        public void Configure(EntityTypeBuilder<FM_CashSweepSettingExt> entity)
        {
            entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

            entity.HasIndex(e => e.NumericalOrder)
                .HasName("index_NumericalOrder");

            entity.HasIndex(e => e.NumericalOrderDetail)
                .HasName("index_NumericalOrderDetail");

            entity.Property(e => e.RecordID)
                .HasComment("主键");

            entity.Property(e => e.AccoSubjectID)
                .HasComment("科目")
                 .AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改时间");

            entity.Property(e => e.NumericalOrder)
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
                .HasComment("详情流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.BusiType)
               .HasComment("业务类型(0:付款 1：收款)");

            entity.Property(e => e.OrganizationSortID)
                .HasComment("业务单元")
                .AddStringToLongConvert();

            entity.Property(e => e.OwnerID)
                .HasComment("制单人")
                .AddStringToLongConvert();

            entity.Property(e => e.ReceiptAbstractID)
                .HasComment("摘要")
                .AddStringToLongConvert();

            entity.Property(e => e.TicketedPointID)
                .HasComment("单据字")
                .AddStringToLongConvert();
        }
    }
}
