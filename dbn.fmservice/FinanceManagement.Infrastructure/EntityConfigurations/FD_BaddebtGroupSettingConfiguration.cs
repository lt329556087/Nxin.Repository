using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_BaddebtGroupSettingConfiguration : IEntityTypeConfiguration<FD_BaddebtGroupSetting>
    {
        public void Configure(EntityTypeBuilder<FD_BaddebtGroupSetting> entity)
        {

            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.DataDate)
          .HasComment("日期");

           // entity.Property(e => e.Number)
           //.HasComment("单据号")
           //.AddStringToLongConvert();

            entity.Property(e => e.StartDate)
         .HasComment("开始日期");

            entity.Property(e => e.EndDate)
           .HasComment("结束日期");

            entity.Property(e => e.Remarks)
         .HasComment("备注");

            entity.Property(e => e.OwnerID)
            .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");

            entity.HasMany(o => o.Details)
            .WithOne(o => o.FD_BaddebtGroupSetting)
             .HasForeignKey(o => o.NumericalOrder)
            .IsRequired();

            entity.HasMany(o => o.Extends)
          .WithOne(o => o.FD_BaddebtGroupSetting)
           .HasForeignKey(o => o.NumericalOrder)
          .IsRequired();
        }
    }

    public class FD_BaddebtGroupSettingDetailConfiguration : IEntityTypeConfiguration<FD_BaddebtGroupSettingDetail>
    {
        public void Configure(EntityTypeBuilder<FD_BaddebtGroupSettingDetail> entity)
        {

            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.BusType)
            .HasComment("类型（0：应收，1：其他应收）");

            entity.Property(e => e.IntervalType)
            .HasComment("类型")
            .AddStringToLongConvert(); 

            entity.Property(e => e.Name)
            .HasComment("名称");

            entity.Property(e => e.DayNum)
          .HasComment("包含天数");

            entity.Property(e => e.Serial)
          .HasComment("序号");

            entity.Property(e => e.ProvisionRatio)
         .HasComment("计提比率");

            entity.Property(e => e.ModifiedDate)
             .HasComment("修改日期");
        }
    }

    public class FD_BaddebtGroupSettingExtendConfiguration : IEntityTypeConfiguration<FD_BaddebtGroupSettingExtend>
    {
        public void Configure(EntityTypeBuilder<FD_BaddebtGroupSettingExtend> entity)
        {

            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.ShowID)
           .HasComment("前端显示ID");

            entity.Property(e => e.EnterpriseID)
            .HasComment("单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.ModifiedDate)
             .HasComment("修改日期");
        }
    }

    public class FD_IdentificationTypeConfiguration : IEntityTypeConfiguration<FD_IdentificationType>
    {
        public void Configure(EntityTypeBuilder<FD_IdentificationType> entity)
        {

            entity.HasKey(e => e.TypeID)
           .HasName("PRIMARY");

            entity.Property(e => e.TypeID)
            .HasComment("认定类型")
            .AddStringToLongConvert();

            entity.Property(e => e.TypeName)
            .HasComment("认定类型名称");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("集团ID")
            .AddStringToLongConvert();

            entity.Property(e => e.BusiType)
            .HasComment("业务类型（0：应收账款 1：其他应收款）");

            entity.Property(e => e.AccrualType)
            .HasComment("计提方式（0：个别认定 1：账龄计提）");

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.OwnerID)
           .HasComment("制单人")
           .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");

            entity.Property(e => e.ModifiedDate)
             .HasComment("修改日期");
        }
    }

    public class FD_IdentificationTypeSubjectConfiguration : IEntityTypeConfiguration<FD_IdentificationTypeSubject>
    {
        public void Configure(EntityTypeBuilder<FD_IdentificationTypeSubject> entity)
        {

            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
           .HasComment("自增ID");

            entity.Property(e => e.TypeID)
            .HasComment("认定类型")
            .AddStringToLongConvert();

            entity.Property(e => e.AccoSubjectID)
            .HasComment("科目ID")
            .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("集团ID")
            .AddStringToLongConvert();

            entity.Property(e => e.IsUse)
            .HasComment("状态（0：停用，1：使用）");

            entity.Property(e => e.DataSourceType)
            .HasComment("数据来源（0：账龄重分类）");

            entity.Property(e => e.OwnerID)
           .HasComment("制单人")
           .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");

            entity.Property(e => e.ModifiedDate)
             .HasComment("修改日期");
        }
    }
}
