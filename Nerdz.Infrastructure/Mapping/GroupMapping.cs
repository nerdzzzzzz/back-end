using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nerdz.Domain.Entities;

namespace Nerdz.Infrastructure.Mapping
{
    public class GroupMapping : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Groups");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Nome)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(g => g.Descricao)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(u => u.GroupProfilePictureUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(g => g.CreatedAt)
                .IsRequired();

            // Um Grupo tem Muitos Usuários
            builder.HasMany(g => g.Users)
                .WithOne(u => u.Group)
                .HasForeignKey(u => u.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
            // Nota: Se deletar o grupo, o GroupId nos usuários vira NULL.

            builder.HasOne(g => g.Admin)
                .WithOne(u => u.AdminOfGroup)
                .HasForeignKey<Group>(g => g.AdminId) // FK está AQUI nesta tabela
                .OnDelete(DeleteBehavior.Restrict);
            // Nota: Restrict impede que o usuário Admin seja deletado enquanto o grupo existir.
        }
    }
}
