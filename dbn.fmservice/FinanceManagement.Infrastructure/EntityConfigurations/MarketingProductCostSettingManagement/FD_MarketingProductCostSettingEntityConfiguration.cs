using System;
using FinanceManagement.Domain.MarketingProductCostSettingManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations.MarketingProductCostSettingManagement
{
    public class FD_MarketingProductCostSettingEntityConfiguration : IEntityTypeConfiguration<FD_MarketingProductCostSetting>
    {
        public void Configure(EntityTypeBuilder<FD_MarketingProductCostSetting> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.HasComment("营销商品成本设定");

            entity.Property(e => e.NumericalOrder)
              .HasColumnType("bigint(20)")
              .HasComment("流水号")
              .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
                .HasColumnType("datetime")
                .HasComment("单据日期");

            //entity.Property(e => e.PeriodBeginDate)
            //    .HasColumnType("date")
            //    .HasComment("会计区间开始日期");

            //entity.Property(e => e.PeriodEndDate)
            //    .HasColumnType("date")
            //    .HasComment("会计区间结束日期");

            entity.Property(e => e.Number)
                .HasColumnType("bigint(20)")
                .HasComment("单据号");
                //.AddStringToLongConvert();

            entity.Property(e => e.AccountingEnterpriseID)
                .HasColumnType("bigint(20)")
                .HasComment("账务单位")
                .HasDefaultValueSql("'0'")
                .AddStringToLongConvert();

            entity.Property(e => e.OwnerID)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment(comment: "制单人")
                .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasComment("创建时间");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasComment("最后修改日期");

            entity.Property(e => e.Remarks)
                .HasColumnType("longtext")
                .HasDefaultValueSql("''")
                .HasComment(comment: "备注");
        }
    }
}
