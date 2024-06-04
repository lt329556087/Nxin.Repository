using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class MS_FormulaDetailConfiguration : IEntityTypeConfiguration<MS_FormulaDetail>
    {
        public void Configure(EntityTypeBuilder<MS_FormulaDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Guid)
            .HasComment("全球唯一关键字");


            entity.Property(e => e.ProductID)
            .HasComment("原料代号ID")
            .AddStringToLongConvert();

            entity.Property(e => e.StockType)
            .HasComment("存货类型")
            .AddStringToLongConvert();

            entity.Property(e => e.FormulaTypeID)
            .HasComment("配方类型ID")
            .AddStringToLongConvert();

            entity.Property(e => e.ProportionQuantity)
            .HasComment("原料耗用数量");

            entity.Property(e => e.Quantity)
            .HasComment("耗用比率");

            entity.Property(e => e.RowNum)
            .HasComment("行号");

            entity.Property(e => e.UnitCost)
            .HasComment("单位成本");

            entity.Property(e => e.Cost)
             .HasComment("成本");

            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");

        }
    }
}
