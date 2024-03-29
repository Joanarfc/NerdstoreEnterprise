﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NSE.Catalog.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Nome = table.Column<string>(type: "varchar(250)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(500)", nullable: false),
                    Ativo = table.Column<bool>(nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataCadastro = table.Column<DateTime>(nullable: false),
                    Imagem = table.Column<string>(type: "varchar(250)", nullable: false),
                    QuantidadeDeStock = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}
