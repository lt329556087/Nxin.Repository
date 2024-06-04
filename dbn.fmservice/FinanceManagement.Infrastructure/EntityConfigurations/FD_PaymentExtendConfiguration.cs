﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure
{
    public class FD_PaymentExtendConfiguration : IEntityTypeConfiguration<FD_PaymentExtend>
    {
        public void Configure(EntityTypeBuilder<FD_PaymentExtend> entity)
        {
            entity.HasKey(e => e.RecordID)
                .HasName("PRIMARY");

            entity.HasComment("付款单收方信息");

            entity.HasIndex(e => e.NumericalOrder)
                .HasName("NumericalOrder");

            entity.HasIndex(e => e.Guid)
                .HasName("idx_Guid");

            entity.Property(e => e.RecordID)
                .HasColumnType("int(11)")
                .HasComment("主键ID");

            entity.Property(e => e.AccountName)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("账户信息")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .HasComment("收款金额");

            entity.Property(e => e.BankAccount)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("银行账户")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.BankDeposit)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("开户银行")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.CollectionId)
                .HasColumnType("bigint(20)")
                .HasComment("收款单位")
                .AddStringToLongConvert();

            entity.Property(e => e.PersonId)
                .HasColumnType("bigint(20)")
                .HasComment("收款人")
                .AddStringToLongConvert();

            entity.Property(e => e.TradeNo)
                .HasColumnType("bigint(20)")
                .HasComment("金融专用流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.Status)
                .HasColumnType("int(11)")
                .HasComment("支付状态");

            entity.Property(e => e.PayResult)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("支付结果")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.TransferType)
                .HasColumnType("int(11)")
                .HasComment("转账方式");

            entity.Property(e => e.PayUse)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("用途")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.IsRecheck)
                .HasColumnType("bit(1)")
                .HasDefaultValueSql("b'0'")
                .HasComment("是否复核,true=复核");

            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.RecheckId)
                .HasColumnType("bigint(20)")
                .HasComment("复核人")
                .AddStringToLongConvert();

            entity.Property(e => e.Guid)
                .HasComment("全球唯一关键字")
                .HasCharSet("utf8")
                .HasCollation("utf8_general_ci");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后修改日期");
        }
    }
}
