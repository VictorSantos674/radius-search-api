# Radius Search API

API REST em .NET para consulta de viabilidade por proximidade geográfica. O serviço recebe uma coordenada e um raio em metros, calcula a distância até equipamentos CTO e retorna os registros elegíveis ordenados pela menor distância.

Este projeto está sendo desenvolvido como parte de um teste técnico para vaga de Desenvolvedor de Software Pleno.

## Objetivo

Disponibilizar um endpoint capaz de responder quais equipamentos do tipo CTO estão dentro de um raio informado, considerando:

- cálculo de distância pela fórmula de Haversine;
- filtragem por status permitidos;
- ordenação por menor distância;
- paginação com limite máximo de 20 itens por página;
- dataset carregado em memória na inicialização da aplicação.

## Stack

- .NET 8
- ASP.NET Core Web API
- C#
- FluentValidation
- Serilog
- xUnit
- FluentAssertions

## Arquitetura

O projeto segue uma organização em camadas, separando responsabilidades de domínio, aplicação, infraestrutura e apresentação.

```text
src/
  RadiusSearch.Api/             # Camada de apresentação HTTP
  RadiusSearch.Application/     # Casos de uso, DTOs e validações
  RadiusSearch.Domain/          # Entidades, value objects e regras de domínio
  RadiusSearch.Infrastructure/  # Repositórios e integrações externas

tests/
  RadiusSearch.Tests/           # Testes unitários
```

Dependências entre camadas:

```text
Api -> Application -> Domain
Api -> Infrastructure -> Application/Domain
Tests -> Domain/Application/Infrastructure
```

## Requisitos Funcionais

Endpoint principal previsto:

```http
GET /api/feasibility?latitude=-23.556456&longitude=-46.635653&radius=100
```

Resposta esperada:

```json
[
  {
    "id": 34,
    "nome": "CTO-RJ-0004",
    "latitude": -23.551,
    "longitude": -46.632,
    "radius": 15.56
  }
]
```

Endpoint de saúde:

```http
GET /health
```

## Validações Previstas

- `latitude`: obrigatório, decimal com no mínimo 5 casas decimais, entre -90 e 90;
- `longitude`: obrigatório, decimal com no mínimo 5 casas decimais, entre -180 e 180;
- `radius`: obrigatório, inteiro, entre 10 e 1000 metros.

## Executando Localmente

Pré-requisitos:

- .NET SDK 8 instalado;
- dataset `dataset_v2.json` disponível no caminho que será configurado na etapa de infraestrutura.

Restaurar dependências:

```powershell
dotnet restore RadiusSearchApi.sln
```

Compilar:

```powershell
dotnet build RadiusSearchApi.sln
```

Executar a API:

```powershell
dotnet run --project src/RadiusSearch.Api/RadiusSearch.Api.csproj
```

Com o perfil local padrão, a API fica disponível em:

```text
http://localhost:5237
```

Verificar saúde da aplicação:

```powershell
curl http://localhost:5237/health
```

## Testes

Executar a suíte de testes:

```powershell
dotnet test RadiusSearchApi.sln
```

Os testes unitários serão adicionados conforme as regras de domínio e aplicação forem implementadas.

## Status do Desenvolvimento

- [x] Estrutura inicial da solution
- [x] Projetos separados por camada
- [x] Configuração inicial de injeção de dependência
- [x] Health check configurado
- [x] Domínio: entidades, value objects e cálculo de distância
- [ ] Infraestrutura: carregamento do dataset em memória
- [ ] Aplicação: caso de uso, DTOs e validações
- [ ] API: endpoint principal e tratamento centralizado de erros
- [ ] Logging com Serilog e rotação de arquivo
- [ ] Testes unitários das regras de negócio
- [ ] Dockerfile
- [ ] Documentação final de deploy

## Formato de Erro Previsto

```json
{
  "code": "400",
  "reason": "empty field",
  "message": "latitude is mandatory",
  "status": "bad request",
  "timestamp": "2025-02-13T14:25:00Z"
}
```

## Headers de Resposta Previstos

- `Content-Type: application/json; charset=utf-8`
- `X-Response-Time: <ms>`
- `X-Request-Id: <UUID gerado na requisição>`

## Licença

Projeto desenvolvido para fins de avaliação técnica.
