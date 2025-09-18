using System.Collections.Generic;
using System.Linq;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Application.Mappers
{
    /// <summary>
    /// Classe utilitária responsável por mapear entidades do tipo <see cref="PessoaJuridica"/> para DTOs.
    /// </summary>
    /// <remarks>
    /// Esta classe implementa o padrão Mapper para converter objetos do domínio em objetos de transferência de dados (DTOs),
    /// facilitando a comunicação entre as camadas da aplicação e a exposição de dados via API sem expor diretamente as entidades do domínio.
    /// Todos os métodos são estáticos para permitir uso direto sem necessidade de instanciação.
    /// </remarks>
    public static class PessoaJuridicaMapper
    {
        /// <summary>
        /// Converte uma entidade <see cref="PessoaJuridica"/> em um DTO <see cref="PessoaJuridicaDto"/>.
        /// </summary>
        /// <param name="pessoa">A entidade de domínio <see cref="PessoaJuridica"/> a ser convertida.</param>
        /// <returns>
        /// Um objeto <see cref="PessoaJuridicaDto"/> contendo os dados da pessoa jurídica e seu endereço.
        /// </returns>
        /// <remarks>
        /// Este método mapeia todas as propriedades relevantes da entidade para o DTO, incluindo 
        /// a conversão do objeto de valor Endereco para EnderecoDto. Campos específicos de pessoa jurídica,
        /// como razão social, nome fantasia e CNPJ são devidamente transferidos.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// Lançada quando o parâmetro <paramref name="pessoa"/> é nulo.
        /// </exception>
        /// <example>
        /// <code>
        /// var pessoaJuridica = await _repository.GetPessoaJuridicaByIdAsync(1);
        /// var dto = PessoaJuridicaMapper.ToDto(pessoaJuridica);
        /// </code>
        /// </example>
        public static PessoaJuridicaDto ToDto(PessoaJuridica pessoa)
        {
            if (pessoa == null)
                throw new System.ArgumentNullException(nameof(pessoa));
                
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

        /// <summary>
        /// Converte uma coleção de entidades <see cref="PessoaJuridica"/> em uma lista de DTOs <see cref="PessoaJuridicaDto"/>.
        /// </summary>
        /// <param name="pessoas">A coleção de entidades <see cref="PessoaJuridica"/> a ser convertida.</param>
        /// <returns>
        /// Uma lista de objetos <see cref="PessoaJuridicaDto"/> contendo os dados das pessoas jurídicas.
        /// </returns>
        /// <remarks>
        /// Este método utiliza LINQ para mapear cada entidade na coleção, chamando o método <see cref="ToDto(PessoaJuridica)"/>
        /// para cada elemento e retornando o resultado como uma lista materializada. A materialização imediata usando ToList()
        /// garante que o mapeamento ocorra de imediato, evitando avaliação tardia.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// Lançada quando o parâmetro <paramref name="pessoas"/> é nulo.
        /// </exception>
        /// <example>
        /// <code>
        /// var todasEmpresas = await _repository.ListPessoaJuridicaAsync();
        /// var dtos = PessoaJuridicaMapper.ToDtoList(todasEmpresas);
        /// </code>
        /// </example>
        public static IEnumerable<PessoaJuridicaDto> ToDtoList(IEnumerable<PessoaJuridica> pessoas)
        {
            if (pessoas == null)
                throw new System.ArgumentNullException(nameof(pessoas));
                
            return pessoas.Select(p => ToDto(p)).ToList();
        }
    }
}