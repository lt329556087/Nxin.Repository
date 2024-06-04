using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FA_PigAssetsResetConfiguration : IEntityTypeConfiguration<FA_PigAssetsReset>
    {
        public void Configure(EntityTypeBuilder<FA_PigAssetsReset> entity)
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
            entity.Property(e => e.TicketedPointID)
            .HasComment("单据字")
            .AddStringToLongConvert();
            entity.Property(e => e.Number)
            .HasComment("单据号")
            .AddStringToLongConvert();

            entity.Property(e => e.PigfarmNatureID)
            .HasComment("猪场性质")
            .AddStringToLongConvert();

            entity.Property(e => e.PigfarmID)
             .HasComment("猪场信息")
            .AddStringToLongConvert();

            entity.Property(e => e.PigNumberTypeId)
             .HasComment("猪数量取值类型")
            .AddStringToLongConvert();

            entity.Property(e => e.PigNumber)
             .HasComment("猪数量");

            entity.Property(e => e.PigPrice)
            .HasComment("重置单价");

            entity.Property(e => e.PigOriginalValue)
            .HasComment("猪场资产重置后原值");

            entity.Property(e => e.BeginAccountPeriodDate)
            .HasComment("开始重置会计期间");

            entity.Property(e => e.EndAccountPeriodDate)
            .HasComment("结束重置会计期间");

            entity.Property(e => e.ResetOriginalValueDate)
            .HasComment("设备重置原值取值期间");

            entity.Property(e => e.ResetOriginalValueType)
            .HasComment("设备重置原值取值类型/重置后原值取值方式")
            .AddStringToLongConvert();

            entity.Property(e => e.EquipmentProportion)
             .HasComment("设备分配比");

            entity.Property(e => e.HouseProportion)
            .HasComment("房屋分配比");

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

    public class FA_PigAssetsResetDetailConfiguration : IEntityTypeConfiguration<FA_PigAssetsResetDetail>
    {
        public void Configure(EntityTypeBuilder<FA_PigAssetsResetDetail> entity)
        {
            entity.HasKey(e => e.NumericalOrderDetail)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
            .HasComment("表体流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.AssetsCode)
            .HasComment("资产编码");

            entity.Property(e => e.InspectNumber)
            .HasComment("资产验收单号");

            entity.Property(e => e.AssetsName)
            .HasComment("资产名称");

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
            
            entity.Property(e => e.MarketID)
            .HasComment("使用部门")
            .AddStringToLongConvert();

            entity.Property(e => e.OriginalValue)
            .HasComment("资产原值");

            entity.Property(e => e.NetValue)
            .HasComment("资产净值");

            entity.Property(e => e.OriginalUseYear)
            .HasComment("原使用期限");

            entity.Property(e => e.ResetBase)
            .HasComment("重置基数");

            entity.Property(e => e.ResetUseYear)
            .HasComment("重置使用期限");

            entity.Property(e => e.ResetOriginalValue)
            .HasComment("重置后原值");

            entity.Property(e => e.ContentType)
            .HasComment("1:设备/2:房屋");

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.CreatedDate)
            .HasComment("创建时间");

            entity.Property(e => e.ModifiedDate)
            .HasComment("修改时间");

        }
    }
}

