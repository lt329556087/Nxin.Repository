using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FM_PerformanceIncomeConfiguration : IEntityTypeConfiguration<FinanceManagement.Domain.FM_PerformanceIncome>
    {
        public void Configure(EntityTypeBuilder<FinanceManagement.Domain.FM_PerformanceIncome> entity)
        {
            entity.HasKey(e => e.RecordID)
            .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("主键");

            entity.Property(e => e.ProductGroupID)
            .HasComment("商品名称ID")
            .AddStringToLongConvert();

            entity.Property(e => e.ProductGroupName)
           .HasComment("商品名称");

            entity.Property(e => e.ProductGroupTypeName)
            .HasComment("商品分类");

            entity.Property(e => e.IncomeTypeName)
            .HasComment("收入分类");

            entity.Property(e => e.ParentTypeName)
             .HasComment("三号文分类");

            entity.Property(e => e.PropertyName)
             .HasComment("匹配属性");

            entity.Property(e => e.EnterpriseID)
            .HasComment("集团ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");
            entity.Property(e => e.ModifiedDate)
            .HasComment("最后修改日期");


        }
    }
}

