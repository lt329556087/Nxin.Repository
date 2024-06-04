using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class MS_FormulaExtendConfiguration : IEntityTypeConfiguration<MS_FormulaExtend>
    {
        public void Configure(EntityTypeBuilder<MS_FormulaExtend> entity)
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
            .HasComment("产品代号ID")
            .AddStringToLongConvert();

            entity.Property(e => e.PackingID)
            .HasComment("包装ID")
            .AddStringToLongConvert();

            entity.Property(e => e.Quantity)
            .HasComment("数量");

            entity.Property(e => e.IsUse)
            .HasComment("状态（0：停用，1：使用）");

            entity.Property(e => e.RowNum)
            .HasComment("行号");

            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");
        }
    }
}
