# 🛒 Plataforma de Gestão de Pedidos

Aplicação estruturada com **Clean Architecture** e **Domain-Driven Design (DDD)**, organizada em **Bounded Contexts** (futuros microsserviços).

## 📐 Arquitetura

```
PedidosPlataforma/
├── Orders/                          # Contexto de Pedidos
│   ├── Orders.Domain/               # ← Núcleo: Entidades, Value Objects, Eventos
│   │   ├── Entities/                #   Pedido
│   │   ├── ValueObjects/            #   ItemPedido, Dinheiro
│   │   ├── Events/                  #   PedidoCriadoEvent, PedidoConfirmadoEvent
│   │   ├── Exceptions/              #   PedidoInvalidoException
│   │   └── Interfaces/              #   IPedidoRepository (Port)
│   ├── Orders.Application/          # ← Casos de uso, DTOs, Interfaces
│   │   ├── UseCases/
│   │   │   ├── CreateOrder/         #   Handler + Command
│   │   │   ├── GetOrder/            #   Handler + Query
│   │   │   └── UpdateOrderStatus/   #   Handler + Command
│   │   ├── DTOs/                    #   Request/Response
│   │   └── Interfaces/              #   IUnitOfWork
│   ├── Orders.Infrastructure/       # ← EF Core, Repositórios (Adapters)
│   │   ├── Persistence/             #   DbContext, Mappings
│   │   └── Repositories/           #   PedidoRepository, UnitOfWork
│   ├── Orders.API/                  # ← Controllers REST, Middleware
│   └── Orders.Tests/                # ← Testes xUnit + FluentAssertions + Moq
│
├── Users/                           # Contexto de Usuários
│   ├── Users.Domain/                #   Entities: Usuario | VO: Credenciais, Endereco
│   ├── Users.Application/
│   ├── Users.Infrastructure/
│   └── Users.API/
│
├── Catalog/                         # Contexto de Catálogo
│   ├── Catalog.Domain/              #   Entities: Produto | VO: Dinheiro
│   └── Catalog.Application/
│
├── Payments/                        # Contexto de Pagamentos
│   ├── Payments.Domain/             #   Entities: Pagamento | Enums: PaymentStatus
│   └── Payments.Application/
│
└── Notifications/                   # Contexto de Notificações
    ├── Notifications.Domain/        #   Entities: Notificacao | Enums: Canal
    └── Notifications.Application/
```

## 🔄 Fluxo de Dependências (Clean Architecture)

```
API → Application → Domain ← Infrastructure
```

- **Domain**: Independente de tudo. Regras de negócio puras.
- **Application**: Depende apenas do Domain. Define interfaces (Ports).
- **Infrastructure**: Implementa as interfaces do Application (Adapters).
- **API**: Ponto de entrada. Depende de Application e Infrastructure.

## 🧪 Testes

```bash
dotnet test Orders/Orders.Tests/Orders.Tests.csproj
```

**Stack de Testes:**
- `xUnit` — framework de testes
- `FluentAssertions` — asserções legíveis
- `Moq` — mocking de dependências

## 🚀 Executar a API de Pedidos

```bash
dotnet run --project Orders/Orders.API/Orders.API.csproj
```

Swagger disponível em: `https://localhost:5001/swagger`
