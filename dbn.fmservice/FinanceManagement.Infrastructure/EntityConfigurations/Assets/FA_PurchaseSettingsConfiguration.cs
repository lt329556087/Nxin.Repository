using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_PurchaseSettingsConfiguration : IEntityTypeConfiguration<FA_PurchaseSettings>
    {
        public void Configure(EntityTypeBuilder<FA_PurchaseSettings> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();
         
            entity.Property(e => e.ModifyFieldID)
            .HasComment("可修改字段")
            .AddStringToLongConvert();
          
            entity.Property(e => e.OwnerID)
             .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();
          
            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");
            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.Main)
            .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();
         
        }
    }

    public class FA_PurchaseSettingsDetailConfiguration : IEntityTypeConfiguration<FA_PurchaseSettingsDetail>
    {
        public void Configure(EntityTypeBuilder<FA_PurchaseSettingsDetail> entity)
        {
            entity.HasKey(e => e.NumericalOrderDetail)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("表体流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.AssetsTypeID)
            .HasComment("资产类别")
            .AddStringToLongConvert();

            entity.Property(e => e.BeginRange)
            .HasComment("开始适用范围");

            entity.Property(e => e.EndRange)
            .HasComment("结束适用范围");

            entity.Property(e => e.FloatingDirectionID)
            .HasComment("浮动方向")
            .AddStringToLongConvert();

            entity.Property(e => e.FloatingTypeID)
            .HasComment("浮动类型")
            .AddStringToLongConvert();

            entity.Property(e => e.MaxFloatingValue)
            .HasComment("最大浮动值");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");

            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");

        }
    }
}

