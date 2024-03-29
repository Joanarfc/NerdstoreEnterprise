﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NSE.Payments.API.Data;

namespace NSE.Payments.API.Migrations
{
    [DbContext(typeof(PagamentosContext))]
    [Migration("20230302145949_Pagamentos")]
    partial class Pagamentos
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.29")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NSE.Payments.API.Models.Pagamento", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PedidoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("TipoPagamento")
                        .HasColumnType("int");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Pagamentos");
                });

            modelBuilder.Entity("NSE.Payments.API.Models.Transacao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BandeiraCartao")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CodigoAutorizacao")
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("CustoTransacao")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("DataTransacao")
                        .HasColumnType("datetime2");

                    b.Property<string>("NSU")
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("PagamentoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TID")
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("ValorTotal")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("PagamentoId");

                    b.ToTable("Transacoes");
                });

            modelBuilder.Entity("NSE.Payments.API.Models.Transacao", b =>
                {
                    b.HasOne("NSE.Payments.API.Models.Pagamento", "Pagamento")
                        .WithMany("Transacoes")
                        .HasForeignKey("PagamentoId")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
