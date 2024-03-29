﻿using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NSE.ShoppingCart.API.Model;
using System.Linq;

namespace NSE.ShoppingCart.API.Data
{
    public class CarrinhoContext : DbContext
    {
        public CarrinhoContext(DbContextOptions<CarrinhoContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<CarrinhoItem> CarrinhoItens { get; set; }
        public DbSet<CarrinhoCliente> CarrinhoCliente { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.Ignore<ValidationResult>();

            modelBuilder.Entity<CarrinhoCliente>()
                .Property(p => p.ValorTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CarrinhoCliente>()
                .Property(c => c.Desconto)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CarrinhoCliente>()
                .HasIndex(c => c.ClienteId)
                .HasDatabaseName("IDX_Cliente");

            modelBuilder.Entity<CarrinhoCliente>()
                .Ignore(c => c.Voucher)
                .OwnsOne(c => c.Voucher, v =>
                {
                    v.Property(vc => vc.Codigo)
                    .HasColumnName("VoucherCodigo")
                    .HasColumnType("varchar(50)");
                    
                    v.Property(vc => vc.TipoDesconto)
                    .HasColumnName("TipoDesconto");
                    v.Property(vc => vc.Percentual)
                    .HasColumnName("Percentual")
                    .HasColumnType("decimal(18,2)");
                    
                    v.Property(vc => vc.ValorDesconto)
                    .HasColumnName("ValorDesconto")
                    .HasColumnType("decimal(18,2)");
                });

            modelBuilder.Entity<CarrinhoCliente>()
                .HasMany(c => c.Itens)
                .WithOne(i => i.CarrinhoCliente)
                .HasForeignKey(c => c.CarrinhoId);

            modelBuilder.Entity<CarrinhoItem>()
                .Property(p => p.Valor)
                .HasColumnType("decimal(18,2)");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }
    }
}