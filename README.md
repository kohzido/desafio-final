# Desafio Final — API REST com Arquitetura Hexagonal

API REST para gestão de **Clientes**, **Produtos** e **Pedidos**, construída em **.NET 9** com arquitetura hexagonal (Ports & Adapters), PostgreSQL e suporte a Docker.

---

## Sumário

- [Estrutura de Pastas](#estrutura-de-pastas)
- [Descrição dos Componentes](#descrição-dos-componentes)
- [Arquitetura](#arquitetura)
  - [C4 — Nível 1: Contexto](#c4--nível-1-contexto)
  - [C4 — Nível 2: Containers](#c4--nível-2-containers)
  - [C4 — Nível 3: Componentes (Hexagonal)](#c4--nível-3-componentes-hexagonal)
  - [Diagrama de Classes — Domínio](#diagrama-de-classes--domínio)
  - [Fluxo de uma Requisição](#fluxo-de-uma-requisição)
- [Como executar](#como-executar)
- [Endpoints](#endpoints)

---

## Estrutura de Pastas

```
desafio-final/
│
├── Domain/                          # Núcleo do negócio — sem dependências externas
│   ├── Entities/                    # Entidades e value objects do domínio
│   │   ├── Cliente.cs
│   │   ├── Produto.cs
│   │   └── Pedido.cs
│   ├── Events/                      # Eventos de domínio
│   │   ├── PedidoCriado.cs
│   │   ├── PedidoAtualizado.cs
│   │   └── PedidoRemovido.cs
│   └── Interfaces/                  # Ports (contratos que o domínio expõe)
│       ├── IClienteRepository.cs
│       ├── IProdutoRepository.cs
│       ├── IPedidoRepository.cs
│       ├── IDomainEvent.cs
│       ├── IDomainEventPublisher.cs
│       └── IEventHandler.cs
│
├── Application/                     # Casos de uso e orquestração
│   ├── UseCases/                    # Um arquivo por operação
│   │   ├── Clientes/                # Create, Read, Update, Delete, Count, GetByName, GetAll
│   │   ├── Produtos/                # Create, Read, Update, Delete, Count, GetByName, GetAll
│   │   └── Pedidos/                 # Create, Read, Update, Delete, Count, GetAll
│   ├── DTOs/                        # Objetos de transferência de dados (Request/Response)
│   ├── Validators/                  # Validações com FluentValidation
│   ├── Facades/                     # Orquestração de operações complexas de Pedido
│   │   ├── PedidoFacade.cs
│   │   ├── PedidoFactory.cs
│   │   └── PedidoValidatorService.cs
│   ├── EventHandlers/               # Handlers dos eventos de domínio
│   └── Exceptions/                  # Exceções de negócio (BusinessRuleException, NotFoundException)
│
├── Infrastructure/                  # Adapters — implementações concretas
│   ├── Database/
│   │   └── AppDbContext.cs          # DbContext do Entity Framework Core
│   ├── Persistence/                 # Implementações dos repositórios (Adapters)
│   │   ├── ClienteRepositoryAdapter.cs
│   │   ├── ProdutoRepositoryAdapter.cs
│   │   └── PedidoRepositoryAdapter.cs
│   ├── Migrations/                  # Migrations do EF Core
│   └── EventPublisher/
│       └── InMemoryDomainEventPublisher.cs
│
├── Api/                             # Camada de apresentação — entrada HTTP
│   ├── Controllers/                 # Controllers REST
│   │   ├── ClienteController.cs
│   │   ├── ProdutoController.cs
│   │   └── PedidoController.cs
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs   # Tratamento global de erros
│   ├── Program.cs                   # Composição do app e injeção de dependências
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── Tests/                           # Testes automatizados
│   ├── UseCases/                    # Testes de casos de uso
│   ├── Validators/                  # Testes de validadores
│   ├── Facades/                     # Testes de facades
│   └── Helpers/                     # Utilitários (MockDbContextHelper)
│
├── Dockerfile                       # Build e runtime da API
├── docker-compose.yml               # Orquestração API + PostgreSQL
└── README.md
```

---

## Descrição dos Componentes

| Componente | Camada | Responsabilidade |
|---|---|---|
| **Entities** (`Domain/Entities`) | Domain | Representam os conceitos centrais do negócio: `Cliente`, `Produto` e `Pedido`. São independentes de qualquer framework. `Cliente` e `Produto` são imutáveis (`record`); `Pedido` é mutável para suportar atualização via EF Core. |
| **Interfaces / Ports** (`Domain/Interfaces`) | Domain | Contratos que o domínio define e espera que sejam implementados externamente. `IClienteRepository`, `IProdutoRepository` e `IPedidoRepository` são as _ports_ de saída. |
| **Domain Events** (`Domain/Events`) | Domain | Eventos que representam fatos ocorridos no domínio (`PedidoCriado`, `PedidoAtualizado`, `PedidoRemovido`). |
| **Use Cases** (`Application/UseCases`) | Application | Encapsulam uma única operação de negócio. Dependem apenas das interfaces do domínio — nunca da infraestrutura diretamente. |
| **DTOs** (`Application/DTOs`) | Application | Objetos de entrada (`Request`) e saída (`Response`) que trafegam entre a API e os casos de uso. Isolam o domínio do contrato HTTP. |
| **Validators** (`Application/Validators`) | Application | Validam os dados de entrada usando FluentValidation. São invocados dentro dos casos de uso como primeira etapa do `ExecuteAsync`, antes de abrir transação ou acionar regras de negócio. |
| **Facades** (`Application/Facades`) | Application | `PedidoFacade` orquestra operações complexas que envolvem múltiplos repositórios e eventos. `PedidoFactory` cria e atualiza objetos `Pedido`. `PedidoValidatorService` verifica existência de entidades relacionadas. |
| **Event Handlers** (`Application/EventHandlers`) | Application | Reagem a eventos de domínio publicados após operações de `Pedido`. |
| **Repository Adapters** (`Infrastructure/Persistence`) | Infrastructure | Implementam os contratos do domínio usando EF Core + PostgreSQL. São os _adapters_ de saída da arquitetura hexagonal. |
| **AppDbContext** (`Infrastructure/Database`) | Infrastructure | Contexto do EF Core. Mapeia entidades para tabelas e configura relacionamentos. |
| **InMemoryDomainEventPublisher** (`Infrastructure/EventPublisher`) | Infrastructure | Publica eventos de domínio em memória, resolvendo handlers via DI. |
| **Controllers** (`Api/Controllers`) | Api | Recebem requisições HTTP, delegam para os casos de uso e retornam respostas. São os _adapters_ de entrada. |
| **ExceptionMiddleware** (`Api/Middleware`) | Api | Captura exceções de negócio (`NotFoundException` → 404, `BusinessRuleException` → 409, `ValidationException` → 400) e retorna JSON padronizado. |
| **Program.cs** | Api | Ponto de entrada. Compõe o container de DI registrando todos os serviços, repositórios, casos de uso e configurações. |

---

## Arquitetura

### C4 — Nível 1: Contexto

```mermaid
C4Context
    title Sistema de Gestão — Contexto

    Person(user, "Usuário / Cliente", "Consome a API via HTTP")

    System(api, "Desafio Final API", "API REST para gestão de Clientes, Produtos e Pedidos")

    System_Ext(postgres, "PostgreSQL", "Banco de dados relacional")

    Rel(user, api, "HTTP/REST (JSON)")
    Rel(api, postgres, "TCP 5432 (Npgsql/EF Core)")
```

---

### C4 — Nível 2: Containers

```mermaid
C4Container
    title Sistema de Gestão — Containers

    Person(user, "Usuário", "Consome a API")

    Container_Boundary(docker, "Docker Compose") {
        Container(api, "API .NET 9", "ASP.NET Core", "Expõe endpoints REST. Porta 5131.")
        ContainerDb(db, "PostgreSQL 16", "Banco de dados", "Persiste Clientes, Produtos e Pedidos. Porta 5432.")
    }

    Rel(user, api, "HTTP/REST", "porta 5131")
    Rel(api, db, "Npgsql / EF Core", "porta 5432 (rede interna Docker)")
```

---

### C4 — Nível 3: Componentes (Hexagonal)

```mermaid
flowchart TD
    subgraph API ["🌐 API (Adapters de Entrada)"]
        direction TB
        CC[ClienteController]
        PC[ProdutoController]
        PedC[PedidoController]
        MW[ExceptionMiddleware]
    end

    subgraph APP ["⚙️ Application (Casos de Uso)"]
        direction TB
        UC_C[Use Cases\nClientes]
        UC_P[Use Cases\nProdutos]
        UC_PED[Use Cases\nPedidos]
        FAC[PedidoFacade\nPedidoFactory\nPedidoValidatorService]
        EH[Event Handlers]
    end

    subgraph DOM ["🔷 Domain (Núcleo)"]
        direction TB
        ENT[Entities\nCliente · Produto · Pedido]
        EVT[Domain Events]
        IFACE[Interfaces / Ports\nIClienteRepository\nIProdutoRepository\nIPedidoRepository\nIDomainEventPublisher]
    end

    subgraph INFRA ["🗄️ Infrastructure (Adapters de Saída)"]
        direction TB
        REPO[Repository Adapters\nClienteRepositoryAdapter\nProdutoRepositoryAdapter\nPedidoRepositoryAdapter]
        CTX[AppDbContext\nEF Core]
        PUB[InMemoryDomainEventPublisher]
    end

    DB[(PostgreSQL)]

    API -->|chama| APP
    APP -->|usa entidades e interfaces| DOM
    INFRA -->|implementa interfaces| DOM
    APP -->|publica eventos| IFACE
    IFACE -.->|implementado por| REPO
    IFACE -.->|implementado por| PUB
    REPO --> CTX
    CTX --> DB
    PUB --> EH
```

> **Regra central da arquitetura hexagonal:** a camada `Domain` não conhece nenhuma das outras. `Application` conhece apenas `Domain`. `Infrastructure` e `Api` conhecem todas as camadas acima, mas nunca são referenciadas por elas.

---

### Diagrama de Classes — Domínio

```mermaid
classDiagram
    direction LR

    class Cliente {
        +Guid Id
        +string Nome
        +string Email
    }

    class Produto {
        +Guid Id
        +string Nome
        +decimal Preco
    }

    class Pedido {
        +Guid Id
        +Guid ClienteId
        +List~Produto~ Produtos
        +decimal ValorTotal
    }

    class IClienteRepository {
        <<interface>>
        +AddAsync(cliente)
        +GetByIdAsync(id)
        +GetAllAsync()
        +GetByNomeAsync(nome)
        +UpdateAsync(cliente)
        +DeleteAsync(id)
        +CountAsync()
        +ExisteEmailAsync(email)
    }

    class IProdutoRepository {
        <<interface>>
        +AddAsync(produto)
        +GetByIdAsync(id)
        +GetAllAsync()
        +GetByNomeAsync(nome)
        +UpdateAsync(produto)
        +DeleteAsync(id)
        +CountAsync()
    }

    class IPedidoRepository {
        <<interface>>
        +AddAsync(pedido)
        +GetByIdAsync(id)
        +GetAllAsync()
        +UpdateAsync(pedido)
        +DeleteAsync(id)
        +CountAsync()
        +ExisteParaClienteAsync(clienteId)
        +ExisteParaProdutoAsync(produtoId)
    }

    class IDomainEvent {
        <<interface>>
    }

    class PedidoCriado {
        +Guid PedidoId
        +Guid ClienteId
        +decimal ValorTotal
    }

    class PedidoAtualizado {
        +Guid PedidoId
        +decimal NovoValorTotal
    }

    class PedidoRemovido {
        +Guid PedidoId
    }

    Pedido "1" --> "1" Cliente : pertence a
    Pedido "1" --> "*" Produto : contém

    IClienteRepository ..> Cliente : gerencia
    IProdutoRepository ..> Produto : gerencia
    IPedidoRepository ..> Pedido : gerencia

    IDomainEvent <|.. PedidoCriado
    IDomainEvent <|.. PedidoAtualizado
    IDomainEvent <|.. PedidoRemovido
```

---

### Fluxo de uma Requisição

```mermaid
sequenceDiagram
    autonumber
    actor User as Usuário
    participant Ctrl as Controller
    participant UC as Use Case
    participant Facade as PedidoFacade
    participant VS as PedidoValidatorService
    participant Repo as Repository Adapter
    participant DB as PostgreSQL
    participant Pub as EventPublisher
    participant EH as EventHandler

    User->>Ctrl: POST /pedidos (JSON)
    Ctrl->>UC: CreatePedidoUseCase.Execute(request)
    UC->>Facade: PedidoFacade.CriarPedidoAsync(clienteId, produtoIds)
    Facade->>VS: ValidarClienteAsync(clienteId)
    VS->>Repo: GetByIdAsync(clienteId)
    Repo->>DB: SELECT
    DB-->>Repo: Cliente
    Repo-->>VS: Cliente
    VS-->>Facade: Cliente
    Facade->>VS: ValidarProdutosAsync(produtoIds)
    VS->>Repo: GetByIdAsync(produtoId) × N
    Repo->>DB: SELECT
    DB-->>Repo: Produto
    Repo-->>VS: Produto
    VS-->>Facade: List~Produto~
    Facade->>Repo: AddAsync(pedido)
    Repo->>DB: INSERT
    DB-->>Repo: ok
    Facade->>Pub: PublishAsync(PedidoCriado)
    Pub->>EH: PedidoCriadoHandler.HandleAsync(event)
    EH-->>Pub: ok
    Facade-->>UC: Pedido
    UC-->>Ctrl: PedidoResponse
    Ctrl-->>User: 201 Created (JSON)
```

---

## Como executar

### Com Docker (recomendado)

```bash
docker-compose up --build
```

- API disponível em: `http://localhost:5131`
- Swagger UI: `http://localhost:5131` (rota raiz)
- PostgreSQL: `localhost:5432`

### Localmente (sem Docker)

Pré-requisitos: .NET 9 SDK e PostgreSQL rodando em `localhost:5432`.

```bash
dotnet restore
dotnet run --project Api
```

---

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/clientes` | Criar cliente |
| `GET` | `/clientes` | Listar todos os clientes |
| `GET` | `/clientes/{id}` | Buscar cliente por ID |
| `GET` | `/clientes/nome/{nome}` | Buscar cliente por nome |
| `GET` | `/clientes/count` | Contar clientes |
| `PUT` | `/clientes/{id}` | Atualizar cliente |
| `DELETE` | `/clientes/{id}` | Remover cliente |
| `POST` | `/produtos` | Criar produto |
| `GET` | `/produtos` | Listar todos os produtos |
| `GET` | `/produtos/{id}` | Buscar produto por ID |
| `GET` | `/produtos/nome/{nome}` | Buscar produto por nome |
| `GET` | `/produtos/count` | Contar produtos |
| `PUT` | `/produtos/{id}` | Atualizar produto |
| `DELETE` | `/produtos/{id}` | Remover produto |
| `POST` | `/pedidos` | Criar pedido |
| `GET` | `/pedidos` | Listar todos os pedidos |
| `GET` | `/pedidos/{id}` | Buscar pedido por ID |
| `GET` | `/pedidos/count` | Contar pedidos |
| `PUT` | `/pedidos/{id}` | Atualizar pedido |
| `DELETE` | `/pedidos/{id}` | Remover pedido |
