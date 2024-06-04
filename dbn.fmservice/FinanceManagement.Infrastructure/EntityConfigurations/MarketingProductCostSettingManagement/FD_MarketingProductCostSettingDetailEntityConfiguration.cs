using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain.MarketingProductCostSettingManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations.MarketingProductCostSettingManagement
{
    public class FD_MarketingProductCostSettingDetailEntityConfiguration : IEntityTypeConfiguration<FD_MarketingProductCostSettingDetail>
    {
        public void Configure(EntityTypeBuilder<FD_MarketingProductCostSettingDetail> entity)
        {
            entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

            entity.HasComment("营销商品成本设定详情");

            entity.Property(e => e.RecordID)
              .HasColumnType("int(11)")
              .HasComment("主键")
              .ValueGeneratedOnAdd();

            entity.Property(e => e.NumericalOrder)
              .HasColumnType("bigint(20)")
              .HasComment("流水号")
              .HasDefaultValueSql("'0'")
              .AddStringToLongConvert();

            entity.Property(e => e.ProductId)
              .HasColumnType("bigint(20)")
              .HasComment("商品ID")
              .HasDefaultValueSql("'0'")
              .AddStringToLongConvert();

            entity.Property(e => e.MeasureUnitNanme)
              .HasColumnType("varchar(128)")
              .HasComment("计量单位名称")
              .HasDefaultValueSql("''");

            entity.Property(e => e.ForecastUnitCost)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValueSql("'0'")
                .HasComment("预测单位成本");

            entity.Property(e => e.CurrentCalcCost)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValueSql("'0'")
                .HasComment("当期计算成本");

            entity.Property(e => e.ForecastAndCurrentDiff)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValueSql("'0'")
                .HasComment("差异值");
        }
    }
}
