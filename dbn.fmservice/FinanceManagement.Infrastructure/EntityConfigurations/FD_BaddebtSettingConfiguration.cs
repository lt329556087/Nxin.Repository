using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_BaddebtSettingConfiguration : IEntityTypeConfiguration<FD_BaddebtSetting>
    {
        public void Configure(EntityTypeBuilder<FD_BaddebtSetting> entity)
        {

            entity.HasKey(e => e.NumericalOrder)
            .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.ProvisionMethod)
           .HasComment("计提方法")
           .AddStringToLongConvert();

            entity.Property(e => e.BadAccoSubjectOne)
           .HasComment("坏账准备科目-应收账款")
           .AddStringToLongConvert();

            entity.Property(e => e.BadAccoSubjectTwo)
           .HasComment("坏账准备科目-其他应收账款")
           .AddStringToLongConvert();

            entity.Property(e => e.OtherAccoSubjectOne)
           .HasComment("减值损失科目-应收账款")
           .AddStringToLongConvert();


            entity.Property(e => e.OtherAccoSubjectTwo)
          .HasComment("减值损失科目-其他应收账款")
          .AddStringToLongConvert();

            entity.Property(e => e.DebtReceAccoSubjectOne)
           .HasComment("应收账款-应收账款")
           .AddStringToLongConvert();

            entity.Property(e => e.DebtReceAccoSubjectTwo)
           .HasComment("应收账款-其他应收账款")
           .AddStringToLongConvert();

            entity.Property(e => e.ReceAccoSubjectOne)
           .HasComment("收款科目-应收账款")
           .AddStringToLongConvert();

            entity.Property(e => e.ReceAccoSubjectTwo)
           .HasComment("收款科目-其他应收账款")
           .AddStringToLongConvert();

            entity.Property(e => e.ProvisionReceiptAbstractID)
           .HasComment("计提坏账摘要")
           .AddStringToLongConvert();

            entity.Property(e => e.OccurReceiptAbstractID)
          .HasComment("坏账发生摘要")
          .AddStringToLongConvert();

            entity.Property(e => e.RecoverReceiptAbstractID)
           .HasComment("坏账收回摘要")
           .AddStringToLongConvert();

            entity.Property(e => e.ReversalReceiptAbstractID)
         .HasComment("计提冲销摘要")
         .AddStringToLongConvert();

            entity.Property(e => e.BadReversalReceiptAbstractID)
           .HasComment("坏账冲销摘要")
           .AddStringToLongConvert();

         //   entity.Property(e => e.GroupNumericalOrder)
         //.HasComment("集团流水号")
         //.AddStringToLongConvert();            

            entity.Property(e => e.OwnerID)
            .HasComment("制单人")
            .AddStringToLongConvert();

            entity.Property(e => e.EnterpriseID)
            .HasComment("所属单位ID")
            .AddStringToLongConvert();

            entity.Property(e => e.CreatedDate)
            .HasComment("创建日期");

            //entity.HasMany(o => o.Details)
            //.WithOne(o => o.FD_BaddebtSetting)
            //.HasForeignKey(o => o.NumericalOrder)
            //.IsRequired();
        }
    }

    public class FD_BaddebtSettingDetailConfiguration : IEntityTypeConfiguration<FD_BaddebtSettingDetail>
    {
        public void Configure(EntityTypeBuilder<FD_BaddebtSettingDetail> entity)
        {

            entity.HasKey(e => e.RecordID)
           .HasName("PRIMARY");

            entity.Property(e => e.RecordID)
            .HasComment("自增ID");

            entity.Property(e => e.NumericalOrder)
            .HasComment("流水号")
            .AddStringToLongConvert();

            entity.Property(e => e.AgingIntervalID)
            .HasComment("账龄区间设置RecordID");

            entity.Property(e => e.ProvisionRatio)
            .HasComment("计提比例");

            entity.Property(e => e.BusType)
            .HasComment("业务类型");

            entity.Property(e => e.Remarks)
            .HasComment("备注");

            entity.Property(e => e.ModifiedDate)
             .HasComment("修改日期");
        }
    }
}
