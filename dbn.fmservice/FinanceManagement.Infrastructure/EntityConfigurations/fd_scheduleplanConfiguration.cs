﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using FinanceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManagement.Infrastructure
{
    public class fd_scheduleplanConfiguration : IEntityTypeConfiguration<fd_scheduleplan>
    {
        public void Configure(EntityTypeBuilder<fd_scheduleplan> entity)
        {
            entity.HasKey(e => e.NumericalOrder)
                .HasName("PRIMARY");

            entity.HasComment("付款排程表");

            entity.Property(e => e.NumericalOrder)
                .HasColumnType("bigint(20)")
                .HasComment("流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.ApplyAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValueSql("'0.00'")
                .HasComment("申请申请金额");

            entity.Property(e => e.ApplyContactType)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("申请往来类型")
                .AddStringToLongConvert();

            entity.Property(e => e.ApplyContactEnterpriseId)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("申请往来单位")
                .AddStringToLongConvert();

            entity.Property(e => e.ApplyData)
                .HasColumnType("date")
                .HasComment("申请日期");

            entity.Property(e => e.ApplyDeadLine)
                .HasColumnType("date")
                .HasComment("申请付款到期日");

            entity.Property(e => e.ApplyEmergency)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("申请是否紧急")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.ApplyEnterpriseId)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("申请单位")
                .AddStringToLongConvert();

            entity.Property(e => e.ApplyMenuId)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("申请单据类型")
                .AddStringToLongConvert();

            entity.Property(e => e.ApplyNumericalOrder)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("申请流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.PayNumericalOrder)
                .HasColumnType("bigint(20)")
                .HasDefaultValueSql("'0'")
                .HasComment("付款单流水号")
                .AddStringToLongConvert();

            entity.Property(e => e.ApplyPayContent)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("申请支出内容")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");

            entity.Property(e => e.ApplySurplusAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValueSql("'0.00'")
                .HasComment("申请剩余金额");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("创建日期");

            entity.Property(e => e.DeadLine)
                .HasColumnType("date")
                .HasComment("计划付款日");

            entity.Property(e => e.GroupId)
                .HasColumnType("bigint(20)")
                .HasComment("集团Id")
                .AddStringToLongConvert();

            entity.Property(e => e.Level)
                .HasColumnType("int(2)")
                .HasComment("付款优先级");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("最后修改日期");

            entity.Property(e => e.OwnerId)
                .HasColumnType("bigint(20)")
                .HasComment("制单人");

            entity.Property(e => e.PayAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValueSql("'0.00'")
                .HasComment("计划支付金额");

            entity.Property(e => e.ScheduleStatus)
                .HasColumnType("int(2)")
                .HasComment("排程状态（0：排程中,1:已排程）");

            entity.Property(e => e.SettlementMethod)
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasComment("结算方式")
                .HasCharSet("utf8mb4")
                .HasCollation("utf8mb4_general_ci");
        }
    }
}
