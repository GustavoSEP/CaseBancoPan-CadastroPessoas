using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Adapters.Input.Api.Models;
using CadastroPessoas.Ports.Input.Commands;
using CadastroPessoas.Ports.Input.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Adapters.Input.Api.Controllers
{
    /// <summary>
    /// API para gerenciamento de pessoas jurídicas
    /// </summary>
    /// <remarks>
    /// Esta API fornece endpoints para operações CRUD (Create, Read, Update, Delete) 
    /// de pessoas jurídicas, incluindo validação de CNPJ e consulta de endereço por CEP.
    /// </remarks>
    [ApiController]
    [Route("api/v1/pessoas/juridicas")]
    public class PessoaJuridicaController : ControllerBase
    {
        private readonly ICreatePessoaJuridicaUseCase _createUseCase;
        private readonly IGetPessoaJuridicaUseCase _getUseCase;
        private readonly IUpdatePessoaJuridicaUseCase _updateUseCase;
        private readonly IDeletePessoaJuridicaUseCase _deleteUseCase;
        private readonly ILogger<PessoaJuridicaController> _logger;

        /// <summary>
        /// Inicializa uma nova instância do controlador de pessoas jurídicas
        /// </summary>
        public PessoaJuridicaController(
            ICreatePessoaJuridicaUseCase createUseCase,
            IGetPessoaJuridicaUseCase getUseCase,
            IUpdatePessoaJuridicaUseCase updateUseCase,
            IDeletePessoaJuridicaUseCase deleteUseCase,
            ILogger<PessoaJuridicaController> logger)
        {
            _createUseCase = createUseCase ?? throw new ArgumentNullException(nameof(createUseCase));
            _getUseCase = getUseCase ?? throw new ArgumentNullException(nameof(getUseCase));
            _updateUseCase = updateUseCase ?? throw new ArgumentNullException(nameof(updateUseCase));
            _deleteUseCase = deleteUseCase ?? throw new ArgumentNullException(nameof(deleteUseCase));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Cadastra uma nova pessoa jurídica
        /// </summary>
        /// <param name="request">Dados da pessoa jurídica a ser cadastrada</param>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST /api/v1/pessoas/juridicas
        ///     {
        ///        "razaoSocial": "Empresa XYZ Ltda.",
        ///        "nomeFantasia": "XYZ Tecnologia",
        ///        "cnpj": "12.345.678/0001-90",
        ///        "cep": "01310-100",
        ///        "numero": "1000",
        ///        "complemento": "Andar 10"
        ///     }
        ///     
        /// O CNPJ pode ser informado com ou sem formatação. O endereço será 
        /// complementado automaticamente com base no CEP informado.
        /// </remarks>
        /// <response code="201">Pessoa jurídica criada com sucesso</response>
        /// <response code="400">Dados inválidos ou CNPJ já cadastrado</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <returns>A pessoa jurídica recém-criada com seu ID gerado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PessoaJuridicaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PessoaJuridicaRequest request)
        {
            try
            {
                var command = new CreatePessoaJuridicaCommand
                {
                    RazaoSocial = request.RazaoSocial,
                    NomeFantasia = request.NomeFantasia,
                    CNPJ = request.CNPJ,
                    CEP = request.CEP,
                    Numero = request.Numero,
                    Complemento = request.Complemento
                };

                var result = await _createUseCase.ExecuteAsync(command);

                var response = new PessoaJuridicaResponse
                {
                    Id = result.Id,
                    RazaoSocial = result.RazaoSocial,
                    NomeFantasia = result.NomeFantasia,
                    CNPJ = result.CNPJ,
                    DataCadastro = result.DataCadastro,
                    Endereco = new EnderecoResponse
                    {
                        Cep = result.Endereco.Cep,
                        Logradouro = result.Endereco.Logradouro,
                        Bairro = result.Endereco.Bairro,
                        Cidade = result.Endereco.Cidade,
                        Estado = result.Endereco.Estado,
                        Numero = result.Endereco.Numero,
                        Complemento = result.Endereco.Complemento
                    }
                };

                return CreatedAtAction(nameof(GetByCnpj), new { cnpj = result.CNPJ }, response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pessoa jurídica");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém uma pessoa jurídica pelo ID
        /// </summary>
        /// <param name="id">ID da pessoa jurídica</param>
        /// <remarks>
        /// Retorna os dados completos de uma pessoa jurídica com base no ID fornecido.
        /// </remarks>
        /// <response code="200">Pessoa jurídica encontrada</response>
        /// <response code="404">Pessoa jurídica não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <returns>Dados da pessoa jurídica</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PessoaJuridicaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pessoa = await _getUseCase.GetByIdAsync(id);

                var response = new PessoaJuridicaResponse
                {
                    Id = pessoa.Id,
                    RazaoSocial = pessoa.RazaoSocial,
                    NomeFantasia = pessoa.NomeFantasia,
                    CNPJ = pessoa.CNPJ,
                    DataCadastro = pessoa.DataCadastro,
                    Endereco = new EnderecoResponse
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

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pessoa jurídica pelo ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém uma pessoa jurídica pelo CNPJ
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica (com ou sem formatação)</param>
        /// <remarks>
        /// Retorna os dados completos de uma pessoa jurídica com base no CNPJ fornecido.
        /// O CNPJ pode ser informado com ou sem formatação (pontos, traço e barra).
        /// </remarks>
        /// <response code="200">Pessoa jurídica encontrada</response>
        /// <response code="404">Pessoa jurídica não encontrada ou CNPJ inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <returns>Dados da pessoa jurídica</returns>
        [HttpGet("cnpj/{cnpj}")]
        [ProducesResponseType(typeof(PessoaJuridicaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCnpj(string cnpj)
        {
            try
            {
                var pessoa = await _getUseCase.GetByCnpjAsync(cnpj);

                var response = new PessoaJuridicaResponse
                {
                    Id = pessoa.Id,
                    RazaoSocial = pessoa.RazaoSocial,
                    NomeFantasia = pessoa.NomeFantasia,
                    CNPJ = pessoa.CNPJ,
                    DataCadastro = pessoa.DataCadastro,
                    Endereco = new EnderecoResponse
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

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pessoa jurídica pelo CNPJ {Cnpj}", cnpj);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Lista todas as pessoas jurídicas cadastradas
        /// </summary>
        /// <remarks>
        /// Retorna uma lista com todas as pessoas jurídicas cadastradas no sistema.
        /// </remarks>
        /// <response code="200">Lista de pessoas jurídicas</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <returns>Lista de pessoas jurídicas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<PessoaJuridicaResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pessoas = await _getUseCase.GetAllAsync();

                var response = new List<PessoaJuridicaResponse>();
                foreach (var pessoa in pessoas)
                {
                    response.Add(new PessoaJuridicaResponse
                    {
                        Id = pessoa.Id,
                        RazaoSocial = pessoa.RazaoSocial,
                        NomeFantasia = pessoa.NomeFantasia,
                        CNPJ = pessoa.CNPJ,
                        DataCadastro = pessoa.DataCadastro,
                        Endereco = new EnderecoResponse
                        {
                            Cep = pessoa.Endereco.Cep,
                            Logradouro = pessoa.Endereco.Logradouro,
                            Bairro = pessoa.Endereco.Bairro,
                            Cidade = pessoa.Endereco.Cidade,
                            Estado = pessoa.Endereco.Estado,
                            Numero = pessoa.Endereco.Numero,
                            Complemento = pessoa.Endereco.Complemento
                        }
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as pessoas jurídicas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa jurídica
        /// </summary>
        /// <param name="id">ID da pessoa jurídica a ser atualizada</param>
        /// <param name="request">Dados a serem atualizados</param>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT /api/v1/pessoas/juridicas/1
        ///     {
        ///        "razaoSocial": "Empresa XYZ Ltda. Atualizada",
        ///        "nomeFantasia": "XYZ Tech",
        ///        "cep": "04538-132",
        ///        "numero": "1500",
        ///        "complemento": "Torre B, 15º andar"
        ///     }
        ///     
        /// A atualização é parcial, ou seja, apenas os campos informados serão atualizados.
        /// O CNPJ não pode ser alterado.
        /// </remarks>
        /// <response code="200">Pessoa jurídica atualizada com sucesso</response>
        /// <response code="404">Pessoa jurídica não encontrada</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <returns>Dados atualizados da pessoa jurídica</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PessoaJuridicaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePessoaJuridicaRequest request)
        {
            try
            {
                var command = new UpdatePessoaJuridicaCommand
                {
                    RazaoSocial = request.RazaoSocial,
                    NomeFantasia = request.NomeFantasia,
                    CEP = request.CEP,
                    Numero = request.Numero,
                    Complemento = request.Complemento
                };

                var result = await _updateUseCase.ExecuteAsync(id, command);

                var response = new PessoaJuridicaResponse
                {
                    Id = result.Id,
                    RazaoSocial = result.RazaoSocial,
                    NomeFantasia = result.NomeFantasia,
                    CNPJ = result.CNPJ,
                    DataCadastro = result.DataCadastro,
                    Endereco = new EnderecoResponse
                    {
                        Cep = result.Endereco.Cep,
                        Logradouro = result.Endereco.Logradouro,
                        Bairro = result.Endereco.Bairro,
                        Cidade = result.Endereco.Cidade,
                        Estado = result.Endereco.Estado,
                        Numero = result.Endereco.Numero,
                        Complemento = result.Endereco.Complemento
                    }
                };

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                if (ex.Message.Contains("não encontrada"))
                    return NotFound(ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pessoa jurídica com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Remove uma pessoa jurídica
        /// </summary>
        /// <param name="id">ID da pessoa jurídica a ser removida</param>
        /// <remarks>
        /// Remove permanentemente uma pessoa jurídica do sistema.
        /// </remarks>
        /// <response code="204">Pessoa jurídica removida com sucesso</response>
        /// <response code="404">Pessoa jurídica não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        /// <returns>Nenhum conteúdo</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _deleteUseCase.ExecuteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pessoa jurídica com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}