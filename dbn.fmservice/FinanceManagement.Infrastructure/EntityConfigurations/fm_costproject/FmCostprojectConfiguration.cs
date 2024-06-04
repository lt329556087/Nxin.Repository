using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FmCostprojectConfiguration : IEntityTypeConfiguration<FmCostproject>
    {
        public void Configure(EntityTypeBuilder<FmCostproject> entity)
        {
            entity.HasKey(e => e.CostProjectId)
                    .HasName("PRIMARY");

            entity.ToTable("fm_costproject");

            entity.Property(e => e.CostProjectId)
                .HasColumnType("bigint(20)")
                .ValueGeneratedNever()
                .HasColumnName("CostProjectID")
                .HasComment("费用项目ID")
                .AddStringToLongConvert();

            entity.Property(e => e.AllocationType)
                .HasColumnType("bigint(20)")
                .HasComment("分摊方式")
                .AddStringToLongConvert();

            entity.Property(e => e.CollectionType)
                .HasColumnType("bigint(20)")
                .HasComment("归集类型")
                .AddStringToLongConvert();

            entity.Property(e => e.CostProjectCode)
                .IsRequired()
                .HasMaxLength(30)
                .HasComment("费用编码");

            entity.Property(e => e.CostProjectName).HasMaxLength(30);

            entity.Property(e => e.CostProjectTypeId)
                .HasColumnType("bigint(20)")
                .HasColumnName("CostProjectTypeID")
                .HasDefaultValueSql("'0'")
                .HasComment("项目分类")
                .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DataDate)
                .HasColumnType("date")
                .HasComment("日期");

            entity.Property(e => e.DataSource)
                .HasColumnType("bigint(20)")
                .HasComment("取数来源")
                .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseId)
                .HasColumnType("bigint(20)")
                .HasColumnName("EnterpriseID")
                .HasComment("单位ID")
                .AddStringToLongConvert();

            entity.Property(e => e.IsUse)
                .HasColumnType("bit(1)")
                .HasDefaultValueSql("b'0'")
                .HasComment("是否启用");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("修改日期");

            entity.Property(e => e.OrderNumber)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'0'")
                .HasComment("序号");

            entity.Property(e => e.OwnerId)
                .HasColumnType("bigint(20)")
                .HasColumnName("OwnerID")
                .HasComment("制单人ID")
                .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
                .HasMaxLength(1024)
                .HasDefaultValueSql("''")
                .HasComment("备注");

            entity.Property(e => e.ModifiedOwnerID)
                .HasColumnType("bigint(20)")
                .HasColumnName("ModifiedOwnerID")
                .HasComment("修改人ID")
                .AddStringToLongConvert();

            entity.Property(e => e.PresetItem)
                .HasColumnType("bigint(20)")
                .HasColumnName("PresetItem")
                .HasComment("绑定预置项")
                .AddStringToLongConvert();
            #region 排除
            //entity.Ignore(e => e.PM_UnifiedPlanDetails);
            #endregion
        }
    }
}
