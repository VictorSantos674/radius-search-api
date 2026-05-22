# radius-search-api

API REST em .NET 8 (C#) para localizacao de equipamentos por raio de distancia geografica.

Desenvolvida com arquitetura em camadas (DDD), Clean Code, FluentValidation, Serilog e suporte a containerizacao via Docker.

---

## Requisitos

| Ferramenta | Versao minima |
|------------|---------------|
| Docker     | 20.10+        |
| .NET SDK   | 8.0 (apenas para rodar sem Docker) |

---

## Deploy com Docker (recomendado)

### 1. Clone o repositorio

```bash
git clone https://github.com/VictorSantos674/radius-search-api.git
cd radius-search-api
```

### 2. Build da imagem

```bash
docker build -t radius-search-api .
```

### 3. Execute o container

```bash
docker run -d \
  --name radius-search-api \
  -p 8080:8080 \
  radius-search-api
```

### 4. Verifique se a API esta no ar

```bash
curl http://localhost:8080/health
```

Resposta esperada: `Healthy` com status `200 OK`.

---

## Deploy sem Docker

### 1. Clone o repositorio

```bash
git clone https://github.com/VictorSantos674/radius-search-api.git
cd radius-search-api
```

### 2. Restaure as dependencias

```bash
dotnet restore RadiusSearchApi.sln
```

### 3. Execute a API

```bash
dotnet run --project src/RadiusSearch.Api --configuration Release --urls http://localhost:5000
```

A API sobe em `http://localhost:5000`.

---

## Uso do endpoint principal

```http
GET /api/feasibility?latitude={lat}&longitude={lon}&radius={metros}
```

### Parametros

| Parametro | Tipo    | Obrigatorio | Restricoes                               |
|-----------|---------|-------------|-------------------------------------------|
| latitude  | float   | Sim         | Entre -90 e 90, minimo 5 casas decimais   |
| longitude | float   | Sim         | Entre -180 e 180, minimo 5 casas decimais |
| radius    | integer | Sim         | Entre 10 e 1000 (metros)                  |
| page      | integer | Nao         | Padrao: 1                                 |
| pageSize  | integer | Nao         | Padrao: 20, maximo: 20                    |

### Exemplo de requisicao

```bash
curl "http://localhost:8080/api/feasibility?latitude=-22.91016&longitude=-43.18298&radius=500"
```

### Exemplo de resposta - `200 OK`

```json
[
  {
    "id": 496,
    "nome": "CTO-RJO-CENTRO-0287",
    "latitude": -22.910281,
    "longitude": -43.182324,
    "radius": 68.52
  }
]
```

### Exemplo de resposta - `400 Bad Request`

```json
{
  "code": "400",
  "reason": "empty field",
  "message": "latitude is mandatory",
  "status": "bad request",
  "timestamp": "2026-05-23T10:00:00Z"
}
```

### Headers retornados

| Header          | Descricao                        |
|-----------------|----------------------------------|
| X-Request-Id    | UUID unico gerado por requisicao |
| X-Response-Time | Tempo de resposta em ms          |

---

## Health check

```bash
curl http://localhost:8080/health
```

---

## Testes

```bash
dotnet test RadiusSearchApi.sln
```

---

## Arquitetura

```text
src/
├── RadiusSearch.Domain         # Entidades, value objects, interfaces, Haversine
├── RadiusSearch.Application    # Use cases, validacoes, DTOs
├── RadiusSearch.Infrastructure # Repositorio in-memory, carregamento do dataset
└── RadiusSearch.Api            # Controllers, middlewares, pipeline HTTP
tests/
└── RadiusSearch.Tests          # Testes unitarios e de integracao
```

---

## Logs

Os logs sao gravados em `logs/api-.log` com rotacao automatica a cada 1MB (maximo 10 arquivos retidos). Sao registrados: request recebida, tempo de execucao e erros.

Ao rodar com Docker, os logs ficam dentro do container. Para persistir externamente:

```bash
docker run -d \
  --name radius-search-api \
  -p 8080:8080 \
  -v $(pwd)/logs:/app/logs \
  radius-search-api
```
