using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InvoiceService.Infrastructure.Data.Mappings;

public class InvoiceMap : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.Id)
            .HasColumnName("Id")
            .IsRequired();
        
        builder.OwnsOne(i => i.Number, number =>
        {
            number.Property(n => n.Value)
                .HasColumnName("Number")
                .HasMaxLength(50)
                .IsRequired();
                
            number.HasIndex(n => n.Value).IsUnique();
        });
        
        var statusConverter = new ValueConverter<EInvoiceStatus, string>(
            v => v.ToString(),
            v => (EInvoiceStatus)Enum.Parse(typeof(EInvoiceStatus), v));

        builder.Property(i => i.Status)
            .HasColumnName("Status")
            .HasConversion(statusConverter)
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(i => i.CustomerId)
            .HasColumnName("CustomerId")
            .IsRequired();
        
        builder.HasOne(i => i.Customer)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Collection de Items (já mapeado no InvoiceItemMap)
        builder.HasMany(i => i.Items)
            .WithOne(ii => ii.Invoice)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(i => i.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(i => i.PrintedAt)
            .HasColumnName("PrintedAt")
            .IsRequired(false);
        
        builder.Property(i => i.Errors)
            .HasColumnName("Errors")
            .HasConversion(
                v => string.Join(';', v), 
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
            )
            .IsRequired(false);
        
        builder.HasIndex(i => i.CustomerId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.CreatedAt);
        builder.HasIndex(i => new { i.CustomerId, i.Status });
    }
}