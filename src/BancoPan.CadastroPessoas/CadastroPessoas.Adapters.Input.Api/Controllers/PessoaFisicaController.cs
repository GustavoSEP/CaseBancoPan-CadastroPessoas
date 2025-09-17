using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
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
    [Route("api/v1/pessoas/fisicas")]
    public class PessoaFisicaController : ControllerBase
    {
        private readonly ICreatePessoaFisicaUseCase _createUseCase;
        private readonly IGetPessoaFisicaUseCase _getUseCase;
        private readonly IUpdatePessoaFisicaUseCase _updateUseCase;
        private readonly IDeletePessoaFisicaUseCase _deleteUseCase;
        private readonly ILogger<PessoaFisicaController> _logger;

        public PessoaFisicaController(
            ICreatePessoaFisicaUseCase createUseCase,
            IGetPessoaFisicaUseCase getUseCase,
            IUpdatePessoaFisicaUseCase updateUseCase,
            IDeletePessoaFisicaUseCase deleteUseCase,
            ILogger<PessoaFisicaController> logger)
        {
            _createUseCase = createUseCase ?? throw new ArgumentNullException(nameof(createUseCase));
            _getUseCase = getUseCase ?? throw new ArgumentNullException(nameof(getUseCase));
            _updateUseCase = updateUseCase ?? throw new ArgumentNullException(nameof(updateUseCase));
            _deleteUseCase = deleteUseCase ?? throw new ArgumentNullException(nameof(deleteUseCase));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [ProducesResponseType(typeof(PessoaFisicaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PessoaFisicaRequest request)
        {
            try
            {
                var command = new CreatePessoaFisicaCommand
                {
                    Nome = request.Nome,
                    CPF = request.CPF,
                    CEP = request.CEP,
                    Numero = request.Numero,
                    Complemento = request.Complemento
                };

                var result = await _createUseCase.ExecuteAsync(command);

                var response = new PessoaFisicaResponse
                {
                    Id = result.Id,
                    Nome = result.Nome,
                    CPF = result.CPF,
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

                return CreatedAtAction(nameof(GetByCpf), new { cpf = result.CPF }, response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pessoa física");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PessoaFisicaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pessoa = await _getUseCase.GetByIdAsync(id);

                var response = new PessoaFisicaResponse
                {
                    Id = pessoa.Id,
                    Nome = pessoa.Nome,
                    CPF = pessoa.CPF,
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
                _logger.LogError(ex, "Erro ao buscar pessoa física pelo ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("cpf/{cpf}")]
        [ProducesResponseType(typeof(PessoaFisicaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCpf(string cpf)
        {
            try
            {
                var pessoa = await _getUseCase.GetByCpfAsync(cpf);

                var response = new PessoaFisicaResponse
                {
                    Id = pessoa.Id,
                    Nome = pessoa.Nome,
                    CPF = pessoa.CPF,
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
                _logger.LogError(ex, "Erro ao buscar pessoa física pelo CPF {Cpf}", cpf);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PessoaFisicaResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pessoas = await _getUseCase.GetAllAsync();

                var response = new List<PessoaFisicaResponse>();
                foreach (var pessoa in pessoas)
                {
                    response.Add(new PessoaFisicaResponse
                    {
                        Id = pessoa.Id,
                        Nome = pessoa.Nome,
                        CPF = pessoa.CPF,
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
                _logger.LogError(ex, "Erro ao buscar todas as pessoas físicas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PessoaFisicaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePessoaFisicaRequest request)
        {
            try
            {
                var command = new UpdatePessoaFisicaCommand
                {
                    Nome = request.Nome,
                    CEP = request.CEP,
                    Numero = request.Numero,
                    Complemento = request.Complemento
                };

                var result = await _updateUseCase.ExecuteAsync(id, command);

                var response = new PessoaFisicaResponse
                {
                    Id = result.Id,
                    Nome = result.Nome,
                    CPF = result.CPF,
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
                _logger.LogError(ex, "Erro ao atualizar pessoa física com ID {Id}", id);
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
                _logger.LogError(ex, "Erro ao excluir pessoa física com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}