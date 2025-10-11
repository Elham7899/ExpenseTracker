﻿using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(t => t.Description)
                   .HasMaxLength(255);

            builder.Property(t => t.Date)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(t => t.Category)
                   .WithMany(c => c.Transactions)
                   .HasForeignKey(t => t.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.User)
                   .WithMany(u => u.Transactions)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
