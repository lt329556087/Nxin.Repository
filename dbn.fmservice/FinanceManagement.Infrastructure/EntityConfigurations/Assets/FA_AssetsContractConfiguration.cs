using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_AssetsContractConfiguration : IEntityTypeConfiguration<FA_AssetsContract>
    {
        public void Configure(EntityTypeBuilder<FA_AssetsContract> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
             .HasComment("数据日期");

            entity.Property(e => e.ContractName)
             .HasComment("合同名称");

            entity.Property(e => e.ContractNumber)
             .HasComment("合同编号")
             .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("申请单位")
            .AddStringToLongConvert();

            entity.Property(e => e.MarketID)
            .HasComment("申请部门")
            .AddStringToLongConvert();

            entity.Property(e => e.SupplierID)
            .HasComment("供应商标识")
            .AddStringToLongConvert();


            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();
            entity.Property(e => e.TicketedPointID)
            .HasComment("单据字")
            .AddStringToLongConvert();

            entity.Property(e => e.ContractTemplate)
            .HasComment("合同模板")
            .AddStringToLongConvert();


            entity.Property(e => e.OwnerID)
             .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.UpDataInfo)
            .HasComment("上传附件信息");

            entity.Property(e => e.ContractClause)
            .HasComment("合同条款");


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

    public class FA_AssetsContractDetailConfiguration : IEntityTypeConfiguration<FA_AssetsContractDetail>
    {
        public void Configure(EntityTypeBuilder<FA_AssetsContractDetail> entity)
        {
            entity.HasKey(e => e.NumericalOrderDetail)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("表体流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderInput)
            .HasComment("申请单表体流水号")
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

            entity.Property(e => e.MeasureUnit)
            .HasComment("计量单位")
            .AddStringToLongConvert();

            entity.Property(e => e.Quantity)
            .HasComment("数量");

            entity.Property(e => e.UnitPrice)
            .HasComment("含税单价");

            entity.Property(e => e.Amount)
            .HasComment("金额");

            entity.Property(e => e.Remarks)
            .HasComment("备注");

        }
    }
}

