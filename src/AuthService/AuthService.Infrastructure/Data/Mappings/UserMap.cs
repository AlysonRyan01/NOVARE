using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Data.Mappings;

public class UserMap :  IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        
        builder.OwnsOne(u => u.Name, fn =>
        {
            fn.Property(f => f.Value)
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsRequired(false);
        });

        builder.OwnsOne(u => u.Email, em =>
        {
            em.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(256)
                .IsRequired();
        });

        builder.OwnsOne(u => u.Password, ph =>
        {
            ph.Property(p => p.Value)
                .HasColumnName("PasswordHash")
                .IsRequired();
        });
        
        builder.OwnsMany(u => u.Roles, roles =>
        {
            roles.WithOwner().HasForeignKey("UserId");

            roles.Property<int>("Id");
            roles.HasKey("Id");

            roles.Property(r => r.Name)
                .HasMaxLength(100)
                .IsRequired();

            roles.Property(r => r.ValidUntil);

            roles.ToTable("UserRoles");
        });
    }
}