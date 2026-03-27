using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Persistence.Mappings;

public class PedidoMapping : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status)
            .HasConversion<string>();

        builder.OwnsMany(p => p.Itens, nav =>
        {
            nav.WithOwner().HasForeignKey("PedidoId");
            nav.Property<int>("Id").ValueGeneratedOnAdd();
            nav.HasKey("PedidoId", "Id");
            nav.Property(i => i.ProdutoId);
            nav.Property(i => i.NomeProduto).HasMaxLength(200);
            nav.Property(i => i.Quantidade);
            nav.Property(i => i.PrecoUnitario);
            nav.Ignore(i => i.Total);
        });

        builder.Ignore(p => p.ValorTotal);
        builder.Ignore(p => p.Eventos);
    }
}
