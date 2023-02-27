using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NSE.Orders.Domain.Vouchers;

namespace NSE.Orders.Infra.Data.Mappings
{
    public class VoucherMapping : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Codigo)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.ToTable("Vouchers");
        }
    }
}