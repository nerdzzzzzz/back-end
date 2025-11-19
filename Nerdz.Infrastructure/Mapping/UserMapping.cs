using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nerdz.Domain.Entities;

namespace Nerdz.Infrastructure.Mapping
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nome)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Sobrenome)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.ProfilePictureUrl)
                .HasMaxLength(500) // URLs do Google costumam ser longas
                .IsRequired(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.LastLoginAt);

            builder.HasOne(u => u.Group)           // Um User tem um Group
                .WithMany(g => g.Users)          // Um Group tem muitos Members (User)
                .HasForeignKey(u => u.GroupId)     // A FK fica no User
                .OnDelete(DeleteBehavior.SetNull); // Se apagar o grupo, o user fica "sem grupo"


            builder.HasOne(u => u.AdminOfGroup)    // User pode ser Admin de um Grupo
                .WithOne(g => g.Admin)             // Group tem obrigatoriamente um Admin
                .HasForeignKey<Group>(g => g.AdminId) // A FK (AdminId) fica na tabela GROUP!
                .OnDelete(DeleteBehavior.Restrict); // Segurança: Não pode apagar User se ele for Admin
        }
    }
}
