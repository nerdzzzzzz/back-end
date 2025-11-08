# Projeto Nerdz.Api

Este projeto é o back-end para o aplicativo mobile Nerdz.
Ele usa .NET 8, Clean Architecture, MediatR e Firebase.

---

## 🚀 Como Rodar o Projeto (Getting Started)

1.  Clone o repositório.
2.  Verifique se você tem o **SDK do .NET 8** instalado.
3.  **[IMPORTANTE] Credenciais do Firebase:**
    * Obtenha o arquivo `.json` da **Conta de Serviço (Service Account)** no console do Firebase.
    * Salve este arquivo em um local seguro (ex: `C:\dev-secrets\nerdz-firebase-key.json`).
    * **Não** comite este arquivo no Git!
4.  **Configurar Segredos (User Secrets):**
    * No Visual Studio, clique com o botão direito no projeto `Nerdz.Api` > "Gerenciar Segredos do Usuário".
    * No `secrets.json` que abrir, adicione a configuração:
    ```json
    {
      "Firebase": {
        "ProjectId": "seu-project-id-do-firebase",
        "CredentialPath": "C:\\dev-secrets\\nerdz-firebase-key.json"
      }
    }
    ```
5.  Execute o projeto `Nerdz.Api` (via `dotnet run` ou Visual Studio).
6.  A API estará rodando em `https://localhost:7123`.
7.  Acesse `https://localhost:7123/swagger` para ver os endpoints.

---

## 🏛️ Arquitetura

O projeto usa **Clean Architecture** para separação de responsabilidades.

* **`Nerdz.Domain` (Núcleo):**
    * Contém as Entidades (ex: `UserProfile`) e as constantes de Domínio (ex: `AppRoles`).
    * Não depende de ninguém.

* **`Nerdz.Application` (Lógica de Negócio):**
    * Contém toda a lógica de negócio usando o padrão **CQRS** com **MediatR**.
    * Define os `Commands` (escritas) e `Queries` (leituras).
    * Define os `Handlers` (a lógica que executa os comandos/queries).
    * Depende apenas do `Domain`.

* **`Nerdz.Infrastructure` (Camada Externa):**
    * Implementa interfaces definidas na `Application`.
    * Contém o código que fala com serviços externos (ex: Firestore, provedores de email, etc.).
    * Depende do `Application`.

* **`Nerdz.Api` (Ponto de Entrada):**
    * API Web ASP.NET Core.
    * Contém os `Controllers`, configuração (`Program.cs`) e middlewares.
    * Os controllers são "magros" (thin) e apenas enviam comandos/queries para o MediatR.
    * Depende do `Application` e `Infrastructure`.

---

## 🔐 Autenticação e Autorização

* **Autenticação:** É 100% delegada ao **Firebase Authentication**. O app mobile (front-end) é responsável por fazer o login (via Google, Apple, Email/Senha) e obter um **JWT**.
* **Validação:** A API **não** loga usuários. Ela apenas **valida** o JWT enviado no header `Authorization: Bearer ...` em todos os endpoints com `[Authorize]`.
* **Autorização (Roles):** Usamos **Firebase Custom Claims**.
    * Um claim `role: "Admin"` no token é mapeado para a Role "Admin" no .NET.
    * Para promover um usuário a Admin, use o endpoint `POST /api/admin/set-role` (que por sua vez exige uma role de Admin).

---

## 🍳 Como Adicionar uma Nova API

(Ex: "Buscar Pedidos do Usuário")

1.  **Domain:** Verifique se a entidade `Pedido` existe em `Nerdz.Domain/Entities`.
2.  **Application:** Crie uma nova *Query* (porque é uma leitura):
    * `Nerdz.Application/Features/Pedidos/Queries/GetMeusPedidos/GetMeusPedidosQuery.cs`
    * `Nerdz.Application/Features/Pedidos/Queries/GetMeusPedidos/GetMeusPedidosQueryHandler.cs`
3.  **Application (Handler):** No `Handler`, injete `ICurrentUserService` (para pegar o `UserId`) e o seu repositório/`FirestoreDb`. Busque os pedidos no banco.
4.  **Api:** Adicione o novo endpoint no `PedidosController.cs`:
    ```csharp
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMeusPedidos()
    {
        var query = new GetMeusPedidosQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }
    ```
