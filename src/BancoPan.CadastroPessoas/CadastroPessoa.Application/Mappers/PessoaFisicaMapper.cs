using System.Collections.Generic;
using System.Linq;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Application.Mappers
{
    /// <summary>
    /// Classe utilitária responsável por mapear entidades do tipo <see cref="PessoaFisica"/> para DTOs.
    /// </summary>
    /// <remarks>
    /// Esta classe implementa o padrão Mapper para converter objetos do domínio em objetos de transferência de dados (DTOs),
    /// facilitando a comunicação entre as camadas da aplicação sem expor diretamente as entidades do domínio.
    /// Todos os métodos são estáticos para permitir uso direto sem necessidade de instanciação.
    /// </remarks>
    public static class PessoaFisicaMapper
    {
        /// <summary>
        /// Converte uma entidade <see cref="PessoaFisica"/> em um DTO <see cref="PessoaFisicaDto"/>.
        /// </summary>
        /// <param name="pessoa">A entidade de domínio <see cref="PessoaFisica"/> a ser convertida.</param>
        /// <returns>
        /// Um objeto <see cref="PessoaFisicaDto"/> contendo os dados da pessoa física e seu endereço.
        /// </returns>
        /// <remarks>
        /// Este método mapeia todas as propriedades relevantes da entidade para o DTO, incluindo 
        /// a conversão do objeto de valor Endereco para EnderecoDto.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// Lançada quando o parâmetro <paramref name="pessoa"/> é nulo.
        /// </exception>
        /// <example>
        /// <code>
        /// var pessoaFisica = await _repository.GetPessoaFisicaByIdAsync(1);
        /// var dto = PessoaFisicaMapper.ToDto(pessoaFisica);
        /// </code>
        /// </example>
        public static PessoaFisicaDto ToDto(PessoaFisica pessoa)
        {
            if (pessoa == null)
                throw new System.ArgumentNullException(nameof(pessoa));
                
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

        /// <summary>
        /// Converte uma coleção de entidades <see cref="PessoaFisica"/> em uma lista de DTOs <see cref="PessoaFisicaDto"/>.
        /// </summary>
        /// <param name="pessoas">A coleção de entidades <see cref="PessoaFisica"/> a ser convertida.</param>
        /// <returns>
        /// Uma lista de objetos <see cref="PessoaFisicaDto"/> contendo os dados das pessoas físicas.
        /// </returns>
        /// <remarks>
        /// Este método utiliza LINQ para mapear cada entidade na coleção, chamando o método <see cref="ToDto(PessoaFisica)"/>
        /// para cada elemento e retornando o resultado como uma lista materializada.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// Lançada quando o parâmetro <paramref name="pessoas"/> é nulo.
        /// </exception>
        /// <example>
        /// <code>
        /// var todasPessoas = await _repository.ListPessoaFisicaAsync();
        /// var dtos = PessoaFisicaMapper.ToDtoList(todasPessoas);
        /// </code>
        /// </example>
        public static IEnumerable<PessoaFisicaDto> ToDtoList(IEnumerable<PessoaFisica> pessoas)
        {
            if (pessoas == null)
                throw new System.ArgumentNullException(nameof(pessoas));
                
            return pessoas.Select(p => ToDto(p)).ToList();
        }
    }
}