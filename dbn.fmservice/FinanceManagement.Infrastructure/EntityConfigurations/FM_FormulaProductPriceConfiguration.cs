using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.QlwCrossDbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.RegularExpressions;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class MS_FormulaProductPriceConfiguration : IEntityTypeConfiguration<MS_FormulaProductPrice>
    {
        public void Configure(EntityTypeBuilder<MS_FormulaProductPrice> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.HasComment("配方商品价格设定");
 
            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.Number)
            .HasColumnType("bigint(20)")
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasComment("创建日期");

            entity.Property(e => e.DataDate)
                .HasColumnType("date")
                .HasComment("日期");

            entity.Property(e => e.GroupID)
                .HasColumnType("bigint(20)")
                .HasComment("集团ID")
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

            entity.HasMany(o => o.Lines)
               .WithOne(o => o.MS_FormulaProductPrice)
               .HasForeignKey(o => o.NumericalOrder)
               .IsRequired();

            entity.HasMany(o => o.ExtList)
              .WithOne(o => o.MS_FormulaProductPrice)
              .HasForeignKey(o => o.NumericalOrder)
              .IsRequired();
        }      
    }

    public class MS_FormulaProductPriceDetailConfiguration : IEntityTypeConfiguration<MS_FormulaProductPriceDetail>
    {
        public void Configure(EntityTypeBuilder<MS_FormulaProductPriceDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();            

            entity.Property(e => e.ProductID)
                .HasColumnType("bigint(20)")
                .HasComment("商品ID")
                .AddStringToLongConvert();

            entity.Property(e => e.Specification)
                .HasComment("规格");

            entity.Property(e => e.StandardPack)
              .HasColumnType("decimal(18,4)")
              .HasComment("标包");

            entity.Property(e => e.MeasureUnit)
                .HasColumnType("bigint(20)")
                .HasComment("计量单位")
                .AddStringToLongConvert();

            entity.Property(e => e.MarketPrice)
               .HasColumnType("decimal(18,4)")
               .HasComment("市场单价");

            entity.Property(e => e.Remarks)
                .HasComment("备注");

            entity.Property(e => e.ModifiedDate)
                .HasComment("最后修改日期");
        }
    }

    public class MS_FormulaProductPriceExtConfiguration : IEntityTypeConfiguration<MS_FormulaProductPriceExt>
    {
        public void Configure(EntityTypeBuilder<MS_FormulaProductPriceExt> entity)
        {
            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.EnterpriseID)
               .HasColumnType("bigint(20)")
               .HasComment("单位ID")
               .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

           

            entity.Property(e => e.ModifiedDate)
               .HasColumnType("timestamp")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .HasComment("最后修改日期")
               .ValueGeneratedOnAddOrUpdate();
        }

    }
}
