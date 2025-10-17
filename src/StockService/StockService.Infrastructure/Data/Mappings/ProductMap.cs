using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockService.Domain.Entities;

namespace StockService.Infrastructure.Data.Mappings;

public class ProductMap : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .IsRequired();
        
        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();
            
            name.HasIndex(n => n.Value);
        });

        builder.OwnsOne(p => p.Description, description =>
        {
            description.Property(d => d.Value)
                .HasColumnName("Description")
                .HasMaxLength(1000)
                .IsRequired();
        });

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(p => p.Value)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            price.HasIndex(p => p.Value);
        });

        builder.OwnsOne(p => p.StockQuantity, stockQuantity =>
        {
            stockQuantity.Property(sq => sq.Value)
                .HasColumnName("StockQuantity")
                .IsRequired();
            
            stockQuantity.HasIndex(sq => sq.Value);
        });
        
        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .IsRequired();
        
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.UpdatedAt);
    }
}