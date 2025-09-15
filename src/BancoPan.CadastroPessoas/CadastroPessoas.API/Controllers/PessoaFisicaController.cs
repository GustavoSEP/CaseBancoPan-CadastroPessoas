using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PessoaCadastro.API.Models.Requests;
using PessoaCadastro.API.Models.Responses;
using PessoaCadastro.Application.Interfaces;
using System;
using CadastroPessoas.Application.Interfaces;

namespace CadastroPessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/pessoas/fisicas")]
    public class PessoasFisicasController : ControllerBase
    {
        private readonly IPessoaFisicaService _service;
    }
}