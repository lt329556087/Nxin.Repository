using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class fm_expensereportConfiguration : IEntityTypeConfiguration<FinanceManagement.Domain.fm_expensereport>
    {
        public void Configure(EntityTypeBuilder<FinanceManagement.Domain.fm_expensereport> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                    .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
                .ValueGeneratedNever()
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DataDate).HasComment("日期");

            entity.Property(e => e.EnterpriseID).HasComment("单位ID")
                .AddStringToLongConvert();

            entity.Property(e => e.ExpenseAmount)
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("费用总金额");

            entity.Property(e => e.ModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.OwnerID).HasComment("制单人ID")
                .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
                .HasDefaultValueSql("''")
                .HasComment("备注");

            entity.Property(e => e.ReportPeriod).HasComment("填报期间");

            #region 排除
            entity.Ignore(e => e.DetailList);
            #endregion
        }
    }
}
