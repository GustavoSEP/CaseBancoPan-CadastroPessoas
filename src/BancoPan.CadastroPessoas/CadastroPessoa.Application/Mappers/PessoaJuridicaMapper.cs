using System.Collections.Generic;
using System.Linq;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Application.Mappers
{
    public static class PessoaJuridicaMapper
    {
        public static PessoaJuridicaDto ToDto(PessoaJuridica pessoa)
        {
            return new PessoaJuridicaDto
            {
                Id = pessoa.Id,
                RazaoSocial = pessoa.RazaoSocial,
                NomeFantasia = pessoa.NomeFantasia,
                CNPJ = pessoa.CNPJ,
                DataCadastro = pessoa.DataCadastro,
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

        public static IEnumerable<PessoaJuridicaDto> ToDtoList(IEnumerable<PessoaJuridica> pessoas)
        {
            return pessoas.Select(p => ToDto(p)).ToList();
        }
    }
}