using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Infrastructure.EntityConfigurations
{
    public class FD_PayextendConfiguration : IEntityTypeConfiguration<FD_Payextend>
    {
        public void Configure(EntityTypeBuilder<FD_Payextend> entity)
        {
            entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.NumericalOrderDetail)
                .HasColumnType("bigint(20)")
                .HasComment("流水号详情")
                .AddStringToLongConvert();

            entity.Property(e => e.BusinessType)
                .HasColumnType("bigint(20)")
                .HasComment("往来类型")
                .AddStringToLongConvert();


            entity.Property(e => e.OrderNo)
                .HasColumnType("bigint(20)")
                .HasComment("订单号")
                .AddStringToLongConvert();

            entity.Property(e => e.PayNO)
               .HasColumnType("varchar(50)")
               .HasComment("支付流水号");

            entity.Property(e => e.ScOrderNo)
                .HasColumnType("varchar(50)")
               .HasComment("业务来源流水号");

            entity.Property(e => e.PayeeID)
               .HasColumnType("bigint(20)")
               .HasComment("收款人")
              .AddStringToLongConvert();

            entity.Property(e => e.PayerID)
               .HasColumnType("bigint(20)")
               .HasComment("付款人")
              .AddStringToLongConvert();


            entity.Property(e => e.PayCardNo)
               .HasComment("付款账号");

            entity.Property(e => e.ReceCardNo)
               .HasComment("收款账号");

            entity.Property(e => e.PayCode)
               .HasComment("支付编码");

            entity.Property(e => e.PayTypeName)
               .HasComment("支付名称");

            entity.Property(e => e.PayTime)
               .HasComment("支付时间");

            entity.Property(e => e.PayStatus)
              .HasComment("支付状态");

            entity.Property(e => e.Remarks)
               .HasColumnType("varchar(1024)")
               .HasComment("备注");

            entity.Property(e => e.Purpose)
               .HasComment("用途");

            entity.Property(e => e.BankRoute)
               .HasComment("汇路");

            entity.Property(e => e.AuditLevel)
               .HasComment("复核级次(0:不复核 1:一级)");

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .HasComment("金额");

            entity.Property(e => e.CreatedDate)
                .HasComment("创建日期");           

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后修改日期")
                .ValueGeneratedOnAddOrUpdate();

          
        }
    }
}
