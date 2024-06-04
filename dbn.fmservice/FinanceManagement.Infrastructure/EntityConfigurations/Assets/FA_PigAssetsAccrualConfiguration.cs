using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_PigAssetsAccrualConfiguration : IEntityTypeConfiguration<FA_PigAssetsAccrual>
    {
        public void Configure(EntityTypeBuilder<FA_PigAssetsAccrual> entity)
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
             .HasComment("计提日期");

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

    public class FA_PigAssetsAccrualDetailConfiguration : IEntityTypeConfiguration<FA_PigAssetsAccrualDetail>
    {
        public void Configure(EntityTypeBuilder<FA_PigAssetsAccrualDetail> entity)
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
           .HasComment("设置单流水号")
           .AddStringToLongConvert();

            entity.Property(e => e.PigfarmNatureID)
            .HasComment("猪场性质")
            .AddStringToLongConvert();

            entity.Property(e => e.PigfarmID)
            .HasComment("猪场信息")
            .AddStringToLongConvert();

            entity.Property(e => e.MarketID)
            .HasComment("使用部门")
            .AddStringToLongConvert();

            entity.Property(e => e.CardCode)
            .HasComment("资产编码");

            entity.Property(e => e.AssetsCode)
            .HasComment("资产编码");

            entity.Property(e => e.AssetsName)
            .HasComment("资产名称");

            entity.Property(e => e.AssetsTypeID)
            .HasComment("资产类别标识")
            .AddStringToLongConvert();

            entity.Property(e => e.OriginalValue)
            .HasComment("原值");

            entity.Property(e => e.DepreciationMonthAmount)
            .HasComment("月折旧额");

            entity.Property(e => e.DepreciationMonthRate)
            .HasComment("月折旧率");
            
            entity.Property(e => e.DepreciationAccumulated)
            .HasComment("累计折旧");

            entity.Property(e => e.NetValue)
            .HasComment("净值");

            entity.Property(e => e.UseMonth)
            .HasComment("使用月份");

            entity.Property(e => e.AlreadyAccruedMonth)
            .HasComment("已经计提月份");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");

            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");

        }
    }
}

