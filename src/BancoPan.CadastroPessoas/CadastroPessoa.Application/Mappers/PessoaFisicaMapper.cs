using System.Collections.Generic;
using System.Linq;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Application.Mappers
{
    public static class PessoaFisicaMapper
    {
        public static PessoaFisicaDto ToDto(PessoaFisica pessoa)
        {
            return new PessoaFisicaDto
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                CPF = pessoa.CPF,
                Endereco = new EnderecoDto
                {
                    Cep = pessoa.Endereco.Cep,
                    Logradouro = pessoa.Endereco.Logradouro,
                    Bairro = pessoa.Endereco.Bairro,
                    Cidade = pessoa.Endereco.Cidade,
                    Estado = pessoa.Endereco.Estado,
                    Numero = pessoa.Endereco.Numero,
                    Complemento = pessoa.Endereco.Complemento
                }
            };
        }

        public static IEnumerable<PessoaFisicaDto> ToDtoList(IEnumerable<PessoaFisica> pessoas)
        {
            return pessoas.Select(p => ToDto(p)).ToList();
        }
    }
}