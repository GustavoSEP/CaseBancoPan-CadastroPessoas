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
    [ApiController]
    [Route("api/v1/pessoas/juridicas")]
    public class PessoaJuridicaController : ControllerBase
    {
        private readonly ICreatePessoaJuridicaUseCase _createUseCase;
        private readonly IGetPessoaJuridicaUseCase _getUseCase;
        private readonly IUpdatePessoaJuridicaUseCase _updateUseCase;
        private readonly IDeletePessoaJuridicaUseCase _deleteUseCase;
        private readonly ILogger<PessoaJuridicaController> _logger;

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