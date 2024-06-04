using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class fm_expensereportdetailConfiguration : IEntityTypeConfiguration<fm_expensereportdetail>
    {
        public void Configure(EntityTypeBuilder<fm_expensereportdetail> entity)
        {
            entity.HasKey(e => e.RecordID)
                    .HasName("PRIMARY");

            entity.Property(e => e.RecordID).HasColumnName("RecordID").HasComment("主键");

            entity.Property(e => e.CollectionType)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CollectionType")
                .HasComment("归集类型")
                .AddStringToLongConvert();

            entity.Property(e => e.CostProjectID)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CostProjectID")
                .HasComment("费用项目ID")
                .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
                .HasColumnName("CreatedDate")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DataSource)
                .HasColumnName("DataSource")
                .HasDefaultValueSql("'0'")
                .HasComment("取数来源")
                .AddStringToLongConvert();

            entity.Property(e => e.AllocationType)
                .HasColumnName("AllocationType")
                .HasDefaultValueSql("'0'")
                .HasComment("分摊方式")
                .AddStringToLongConvert();

            entity.Property(e => e.ExpenseAmount)
                .HasColumnName("ExpenseAmount")
                .HasPrecision(18, 4)
                .HasDefaultValueSql("'0.0000'")
                .HasComment("费用金额");

            entity.Property(e => e.IsCollect).HasColumnName("IsCollect").HasComment("是否归集");

            entity.Property(e => e.ModifiedDate)
                .HasColumnName("ModifiedDate")
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.NumericalOrder).HasColumnName("NumericalOrder").HasComment("流水号")
                .AddStringToLongConvert();

            #region 排除
            entity.Ignore(e => e.ExtendList);
            entity.Ignore(e => e.ExtendDetailList);
            entity.Ignore(e => e.DetailLogList);
            #endregion
        }
    }
}
