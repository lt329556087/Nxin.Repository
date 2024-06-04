using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_AssetsInspectConfiguration : IEntityTypeConfiguration<FA_AssetsInspect>
    {
        public void Configure(EntityTypeBuilder<FA_AssetsInspect> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();
         
            entity.Property(e => e.EnterpriseID)
            .HasComment("申请单位")
            .AddStringToLongConvert();
          
            entity.Property(e => e.DataDate)
             .HasComment("数据日期");

            entity.Property(e => e.TicketedPointID)
            .HasComment("单据字")
            .AddStringToLongConvert();

            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.MarketID)
             .HasComment("申请部门")
            .AddStringToLongConvert();

            entity.Property(e => e.PersonID)
             .HasComment("经办人")
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

    public class FA_AssetsInspectDetailConfiguration : IEntityTypeConfiguration<FA_AssetsInspectDetail>
    {
        public void Configure(EntityTypeBuilder<FA_AssetsInspectDetail> entity)
        {
            entity.HasKey(e => e.NumericalOrderDetail)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("表体流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.AssetsName)
            .HasComment("资产标识");

            entity.Property(e => e.AssetsTypeID)
            .HasComment("资产类别标识")
            .AddStringToLongConvert();

            entity.Property(e => e.Specification)
            .HasComment("规格型号");

            entity.Property(e => e.Brand)
            .HasComment("品牌");

            entity.Property(e => e.AssetsNatureId)
            .HasComment("资产性质")
            .AddStringToLongConvert();

            entity.Property(e => e.MeasureUnit)
            .HasComment("计量单位")
            .AddStringToLongConvert();

            entity.Property(e => e.Quantity)
            .HasComment("数量");


            entity.Property(e => e.UnitPrice)
            .HasComment("单价");

            entity.Property(e => e.Amount)
            .HasComment("金额");

            entity.Property(e => e.SupplierID)
            .HasComment("供应商标识")
            .AddStringToLongConvert();

            entity.Property(e => e.ProjectID)
            .HasComment("项目标识")
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

