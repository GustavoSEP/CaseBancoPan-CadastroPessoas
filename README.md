# Sistema de Cadastro de Pessoas - Desafio Técnico - Banco Pan

![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blue)
![C#](https://img.shields.io/badge/C%23-10.0-brightgreen)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-blueviolet)
![Swagger](https://img.shields.io/badge/Swagger-3.0-green)

Projeto desenvolvido como parte de uma entrevista técnica para o Banco Pan, visando a candidatura à vaga de Engenheiro .NET Júnior. O objetivo foi demonstrar habilidades práticas em desenvolvimento de APIs e boas práticas de arquitetura.

## 📋 Índice

- [Sistema de Cadastro de Pessoas - Desafio Técnico - Banco Pan](#sistema-de-cadastro-de-pessoas---desafio-técnico---banco-pan)
  - [📋 Índice](#-índice)
  - [🔍 Visão Geral](#-visão-geral)
  - [🏗️ Arquitetura Clean Architecture](#️-arquitetura-clean-architecture)
    - [Estrutura de Pastas](#estrutura-de-pastas)
    - [Camadas](#camadas)
  - [🚀 Funcionalidades](#-funcionalidades)
    - [Gerenciamento de Pessoas Físicas](#gerenciamento-de-pessoas-físicas)
    - [Gerenciamento de Pessoas Jurídicas](#gerenciamento-de-pessoas-jurídicas)
    - [Validação de Documentos](#validação-de-documentos)
    - [Consulta de Endereços](#consulta-de-endereços)
  - [💻 Tecnologias Utilizadas](#-tecnologias-utilizadas)
  - [⚙ Configuração e Execução](#-configuração-e-execução)
    - [Pré-requisitos](#pré-requisitos)
    - [Configuração do Banco de Dados](#configuração-do-banco-de-dados)
    - [Execução da Aplicação](#execução-da-aplicação)
  - [📡 Endpoints da API](#-endpoints-da-api)
    - [Pessoas Físicas](#pessoas-físicas)
    - [Pessoas Jurídicas](#pessoas-jurídicas)
  - [🔌 Integração com Serviços Externos](#-integração-com-serviços-externos)
    - [ViaCEP](#viacep)
  - [🧪 Testes](#-testes)
    - [Testes de Serviços](#testes-de-serviços)
    - [Testes de Repositórios](#testes-de-repositórios)
    - [Execução dos Testes](#execução-dos-testes)
  - [📚 Documentação](#-documentação)
  - [👨‍💻 Autor](#-autor)

## 🔍 Visão Geral

O Sistema de Cadastro de Pessoas é uma API que permite o gerenciamento completo de registros de pessoas físicas e jurídicas. O sistema possibilita operações CRUD (Create, Read, Update, Delete) para ambos os tipos de entidades, com validação de documentos brasileiros (CPF e CNPJ) e integração com serviços externos para enriquecimento de dados.


## 🏗️ Arquitetura Clean Architecture

Este projeto implementa a **Clean Architecture**, com ênfase na separação entre regras de negócio e detalhes técnicos.


### Estrutura de Pastas

```
BancoPan.CadastroPessoas/
├── CadastroPessoas.Domain/               # Entidades e interfaces do domínio
│   ├── Entities/                         # Entidades de domínio (PessoaFisica, PessoaJuridica, Endereco)
│   └── Interfaces/                       # Contratos de repositório e serviços
├── CadastroPessoa.Application/           # Lógica de aplicação
│   ├── Helpers/                          # Classes utilitárias (DocumentoHelper)
│   ├── Interfaces/                       # Contratos dos serviços de aplicação
│   └── Services/                         # Implementações dos serviços de aplicação
├── CadastroPessoas.Infrastructure.SQL/   # Implementações de infraestrutura
│   ├── Data/                             # Contexto de banco de dados e configurações
│   ├── Repositories/                     # Implementações dos repositórios
│   └── Services/                         # Serviços de infraestrutura (ViaCepService)
├── CadastroPessoas.API/                  # Camada de apresentação e API
│   ├── Controllers/                      # Controladores da API
│   ├── Models/                           # DTOs para requests e responses
│   └── Program.cs                        # Configuração da aplicação
└── CadastroPessoas.Tests/                # Testes unitários
    ├── Infrastructure/                   # Testes de componentes de infraestrutura
    └── Application/                      # Testes de componentes de aplicação
```

### Camadas

1. **Domínio**: Contém as entidades centrais do negócio (PessoaFisica, PessoaJuridica, Endereco) e contratos de repositório. É independente de frameworks e implementações específicas.

2. **Aplicação**: Contém a lógica de negócio, implementando os casos de uso da aplicação. Depende apenas do domínio e orquestra operações entre entidades e serviços.

3. **Infraestrutura**: Implementa os contratos definidos no domínio, como repositórios de banco de dados (com Entity Framework Core) e integrações externas (serviço ViaCEP).

4. **API**: Camada de apresentação que expõe os endpoints REST, gerencia requisições, validações e respostas.

5. **Testes**: Implementação de testes unitários e de integração para verificar o comportamento do sistema.

## 🚀 Funcionalidades

### Gerenciamento de Pessoas Físicas
- Cadastro de pessoas físicas com validação de CPF
- Consulta de pessoas físicas por CPF
- Listagem de todas as pessoas físicas cadastradas
- Atualização de dados de pessoas físicas
- Exclusão de pessoas físicas

### Gerenciamento de Pessoas Jurídicas
- Cadastro de pessoas jurídicas com validação de CNPJ
- Consulta de pessoas jurídicas por CNPJ
- Listagem de todas as pessoas jurídicas cadastradas
- Atualização de dados de pessoas jurídicas
- Exclusão de pessoas jurídicas

### Validação de Documentos
- Validação de CPF usando algoritmo oficial
- Validação de CNPJ usando algoritmo oficial
- Formatação padronizada de documentos (000.000.000-00 para CPF e 00.000.000/0000-00 para CNPJ)
- Normalização de documentos para facilitar pesquisas e comparações

### Consulta de Endereços
- Integração com a API ViaCEP para obtenção automática de dados de endereço a partir do CEP
- Cache de consultas para melhorar performance e reduzir chamadas à API externa
- Políticas de retry e circuit breaker para lidar com falhas na integração externa

## 💻 Tecnologias Utilizadas

- **.NET 8.0**: Framework base para o desenvolvimento da aplicação
- **ASP.NET Core**: Para criação da API RESTful
- **Entity Framework Core**: ORM para acesso ao banco de dados
- **SQL Server**: Banco de dados relacional para persistência
- **Swagger/OpenAPI**: Documentação interativa da API
- **Polly**: Implementação de padrões de resiliência para integrações externas
- **xUnit**: Framework para testes unitários
- **Moq**: Framework para criação de mocks em testes
- **MemoryCache**: Cache em memória para dados frequentemente acessados

## ⚙ Configuração e Execução

### Pré-requisitos
- SDK .NET 8.0 ou superior
- SQL Server (ou LocalDB)
- Visual Studio 2022 ou outro editor compatível com C# (VS Code, Rider)

### Configuração do Banco de Dados
1. Abra o arquivo `appsettings.json` e configure a string de conexão:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SeuServidor;Database=CadastroPessoa;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;Encrypt=True"
}
```

2. Execute as migrações do Entity Framework Core para criar o banco de dados:
```powershell
# Navegue até o diretório da API
cd src/BancoPan.CadastroPessoas/CadastroPessoas.API

# Instale o dotnet-ef
dotnet tool install --global dotnet-ef

# Crie a migration
dotnet ef migrations add NomeDaMigration

# Execute as migration
dotnet ef database update
```

### Execução da Aplicação
```powershell
# Navegue até o diretório da API
cd src/BancoPan.CadastroPessoas/CadastroPessoas.API

# Execute a aplicação
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `https://localhost:5001/index.html`

## 📡 Endpoints da API

### Pessoas Físicas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/pessoas/fisicas` | Cadastra uma nova pessoa física |
| GET | `/api/v1/pessoas/fisicas` | Lista todas as pessoas físicas |
| GET | `/api/v1/pessoas/fisicas/{cpf}` | Busca uma pessoa física pelo CPF |
| PUT | `/api/v1/pessoas/fisicas/{cpf}` | Atualiza uma pessoa física |
| DELETE | `/api/v1/pessoas/fisicas/{cpf}` | Remove uma pessoa física |

### Pessoas Jurídicas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/pessoas/juridicas` | Cadastra uma nova pessoa jurídica |
| GET | `/api/v1/pessoas/juridicas` | Lista todas as pessoas jurídicas |
| GET | `/api/v1/pessoas/juridicas/{cnpj}` | Busca uma pessoa jurídica pelo CNPJ |
| PUT | `/api/v1/pessoas/juridicas/{cnpj}` | Atualiza uma pessoa jurídica |
| DELETE | `/api/v1/pessoas/juridicas/{cnpj}` | Remove uma pessoa jurídica |

## 🔌 Integração com Serviços Externos

### ViaCEP

O sistema integra-se com a API pública ViaCEP para obter automaticamente informações de endereço a partir do CEP. A integração implementa:

- **Cache**: Endereços consultados são armazenados em cache por 6 horas para reduzir chamadas à API externa
- **Resiliência**: Políticas de retry para lidar com falhas temporárias na rede
- **Timeout**: Limite de tempo para respostas da API externa (6 segundos)

## 🧪 Testes

O projeto inclui testes unitários para verificar o comportamento dos componentes principais:

### Testes de Serviços
- Testes para o serviço ViaCEP
- Testes para serviços de Pessoa Física e Jurídica

### Testes de Repositórios
- Testes para os repositórios SQL

### Execução dos Testes
```powershell
# Navegue até o diretório de testes
cd src/BancoPan.CadastroPessoas/CadastroPessoas.Tests

# Execute os testes
dotnet test
```

## 📚 Documentação

O sistema possui uma documentação detalhada através do Swagger/OpenAPI, que pode ser acessada pela interface do Swagger UI quando a aplicação estiver em execução.

A documentação inclui:
- Descrição detalhada de cada endpoint
- Exemplos de requisições e respostas
- Schemas dos objetos utilizados
- Códigos de status possíveis para cada operação

Além disso, o código está documentado com comentários XML, fornecendo informações sobre:
- Propósito das classes e interfaces
- Funcionalidade dos métodos
- Descrição dos parâmetros
- Exceções que podem ser lançadas
- Observações específicas sobre comportamentos

## 👨‍💻 Autor

**Gustavo Moreira Santana**
- Email: gustamoreira26@gmail.com
- GitHub: [GustavoSEP](https://github.com/GustavoSEP)