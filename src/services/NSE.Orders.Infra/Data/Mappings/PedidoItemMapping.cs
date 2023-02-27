using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NSE.Orders.Domain.Pedidos;

namespace NSE.Orders.Infra.Data.Mappings
{
    public class PedidoItemMapping : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ProdutoNome)
                .IsRequired()
                .HasColumnType("varchar(250)");

            // 1 : N => Pedido : PedidoItens
            builder.HasOne(c => c.Pedido)
                .WithMany(c => c.PedidoItens);

            builder.ToTable("PedidoItens");
        }
    }
}