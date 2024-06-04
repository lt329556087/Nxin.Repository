using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_AssetsMaintainConfiguration : IEntityTypeConfiguration<FA_AssetsMaintain>
    {
        public void Configure(EntityTypeBuilder<FA_AssetsMaintain> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();
         
            entity.Property(e => e.EnterpriseID)
            .HasComment("单位标识")
            .AddStringToLongConvert();
          
            entity.Property(e => e.DataDate)
             .HasComment("数据日期");

            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.OwnerID)
             .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");

            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.Main)
            .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();
         
        }
    }

    public class FA_AssetsMaintainDetailConfiguration : IEntityTypeConfiguration<FA_AssetsMaintainDetail>
    {
        public void Configure(EntityTypeBuilder<FA_AssetsMaintainDetail> entity)
        {
            entity.HasKey(e => e.NumericalOrderDetail)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("表体流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.CardID)
            .HasComment("资产ID")
            .AddStringToLongConvert();

            entity.Property(e => e.AssetsCode)
            .HasComment("资产编码");

            entity.Property(e => e.AssetsName)
            .HasComment("资产名称");

            entity.Property(e => e.MaintainID)
            .HasComment("保养方式")
            .AddStringToLongConvert();

            entity.Property(e => e.MaintainDate)
            .HasComment("保养时间");

            entity.Property(e => e.Content)
            .HasComment("内容");

            entity.Property(e => e.Amount)
            .HasComment("金额");

            entity.Property(e => e.DepositID)
            .HasComment("存放地点ID")
            .AddStringToLongConvert();

            entity.Property(e => e.FileName)
            .HasComment("文件名");

            entity.Property(e => e.FilePath)
            .HasComment("文件路径");

            entity.Property(e => e.PersonID)
            .HasComment("责任人")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");

            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");
        }
    }
}

