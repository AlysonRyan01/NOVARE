using InvoiceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceService.Infrastructure.Data.Mappings;

public class InvoiceItemMap : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");
        
        builder.HasKey(ii => new { ii.InvoiceId, ii.ProductId });
        
        builder.Property(ii => ii.InvoiceId)
            .IsRequired();
        
        builder.Property(ii => ii.ProductId)
            .IsRequired();

        builder.Property(ii => ii.ProductName)
            .HasColumnName("ProductName")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ii => ii.Quantity)
            .HasColumnName("Quantity")
            .IsRequired();

        builder.Property(ii => ii.UnitPrice)
            .HasColumnName("UnitPrice")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.HasOne(ii => ii.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(ii => ii.ProductId);
        builder.HasIndex(ii => ii.InvoiceId);
    }
}