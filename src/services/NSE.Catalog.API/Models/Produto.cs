﻿using NSE.Core.DomainObjects;
using System;

namespace NSE.Catalog.API.Models
{
    public class Produto : Entity, IAggregateRoot
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Imagem { get; set; }
        public int QuantidadeDeStock { get; set; }
        public void RetirarStock(int quantidade)
        {
            if (QuantidadeDeStock >= quantidade)
                QuantidadeDeStock -= quantidade;
        }
        public bool EstaDisponivel(int quantidade)
        {
            return Ativo && QuantidadeDeStock >= quantidade;
        }
    }
}