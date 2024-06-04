using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class MS_FormulaConfiguration : IEntityTypeConfiguration<FinanceManagement.Domain.MS_Formula>
    {
        public void Configure(EntityTypeBuilder<FinanceManagement.Domain.MS_Formula> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.Guid)
           .HasComment("全球唯一关键字");

            entity.Property(e => e.TicketedPointID)
           .HasComment("字号（开票点）")
           .AddStringToLongConvert();

            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
            .HasComment("日期");

            entity.Property(e => e.FormulaName)
            .HasComment("配方名称");

            entity.Property(e => e.IsUse)
            .HasComment("状态（0：停用，1：使用）");

            entity.Property(e => e.BaseQuantity)
            .HasComment("配方基数");

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.PackageRemarks)
            .HasComment("包装信息备注");

            entity.Property(e => e.OwnerID)
             .HasComment("制单人ID")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.EarlyWarning)
            .HasComment("预警值");


            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");
            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");

            entity.Property(e => e.UseEnterprise)
           .HasComment("应用单位ID")
           .AddStringToLongConvert();

            entity.Property(e => e.UseProduct)
        .HasComment("使用产品ID")
        .AddStringToLongConvert();

            entity.Property(e => e.EffectiveBeginDate)
            .HasComment("有效开始时间");

            entity.Property(e => e.EffectiveEndDate)
           .HasComment("有效结束时间");

            entity.Property(e => e.IsGroup)
           .HasComment("是否集团（0单位,1集团）");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.MS_Formula)
            .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();

            entity.HasMany(o => o.Extends)
           .WithOne(o => o.MS_Formula)
           .HasForeignKey(o => o.NumericalOrder)
           .IsRequired();
        }
    }
}

