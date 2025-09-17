# Estudos sobre .NET (ASP.NET Core)

Anotações e conceitos importantes sobre o ecossistema .NET.

# Fundamentos do .NET

Objetivo: entender a estrutura e ciclo de vida de uma aplicação .NET.

## Program.cs

- 🔵 O `Program.cs` é como montar uma lanchonete. Digamos que eu queira montar um lanchonete (aplicação .NET) mas precisamos seguir alguns passos antes de abrir as portas e atender a clientela (requisições HTTP).
- O `Program.cs` é o ponto de partida de uma aplicação .NET. Aqui nós configuramos e executamos uma aplicação web.

Exemplo de Estruturá Básica:
```csharp
/* Program.cs */
var builder = WebApplication.CreateBuilder(args);

// 1. Configuração de serviços (DI, CORS, EF Core, Identity, etc.)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Configuração do pipeline HTTP (Middlewares)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```
- `var builder = WebApplication.CreateBuilder(args);`
    - 🔵 É como chamar um arquiteto e começar a planejar uma lanchonete. O arquiteto pega seu plano (configurações em `appsettings.json`, ambiente, etc) e começa a preparar o projeto.
    - Cria um **builder** que inicializa a aplicação com configurações padrão.
    - Lê `appsettings.json`, variáveis de ambiente, argumentos de linhas de comando (CLI), etc.
- `builder.Services.AddXXXX()`
    -  🔵 Agora precisamos contratar uma equipe e comprar equipamentos para a lanchonete.
    - Aqui você registra serviços no container de injeção de dependência (`AddControllers()` - para MVC/API; `AddDbContext()` - para Entity Framework; `AddCors()` - para segurança entre domínios, `AddScoped`, `AddSingleton`, `AddTransient` para seus próprios serviços...).
- `builder.Build()`
    - 🔵 Agora que está tudo pronto no papel, podemos construir a lanchonete. É tipo apertar um botão mágico de montar toda a estrutura do papel para que fique pronta para abrirmos as portas da lanchonete!
    - Compila as configurações e constrói o objeto WebApplication, que agora está pronto para ser executado.
- Configuração do pipeline (Middlewares)
    - 🔵 Aqui nos definimos qual o caminho o cliente deve seguir dentro da lanchonete. Portanto, podemos imaginar a seguinte situação: 1. Um segurança verifica se ele tem máscara (HTTPS Redirection). 2. Um atendente confere se ele tem ticket de desconto (Autenticação). 3. Outro verifica se ele pode acessar certas áreas (Autorização). 4. Aí ele vai pro caixa e faz o pedido (Controller). 5. O pedido é enviado pra cozinha e a comida volta pra ele (Resposta HTTP).

```csharp

app.UseHttpsRedirection(); // 🔵 Segurança na entrada - Redireciona HTTP para HTTPS
app.UseCors(); // Aplica política de CORS
app.UseAuthentication(); // 🔵 Checa identidade - Middleware de autenticação
app.UseAuthorization(); // 🔵 Verifica permissões - Middleware de autorização
app.MapControllers(); // 🔵 Onde os pedidos são atendidos - Mapeia os endpoints dos controllers
```
- `app.Run();`
    - 🔵 Finalmente... portas abertas! Lanchonete está funcionando!
    - Inicia a aplicação ASP.NET Core e escuta requisições HTTP.

## Explicando appsettings.json do .NET

`appsettings.json` é o arquivo de configuração padrão do .NET que armazena <ins>chave-valor</ins> em formato `JSON`. Este arquivo é lido automaticamente quando executamos uma aplicação do .NET.

Algumas informações comuns que podemos encontrar/inserir lá são:
- String de conexão com banco de dados.
- Chaves API externas
- Configurações para cada ambiente (Development, Production...)

## Ciclo de Vida de uma Aplicação .NET

Para facilitar a leitura a seguir, veja o fluxo exemplo abaixo:
```
Início → Program.cs → Middlewares → Routing → Controller/Action → Resposta
```

O ciclo de vida de uma aplicação .NET se refere ao processo que acontece desde a inicialização do projeto até o momento em que uma requisição HTTP é recebida, processada e uma resposta é enviada ao cliente.

1. Inicialmente, a execução da aplicação começa pelo `Program.cs`.
2. Após a inicialização, temos a etapa de **middlewares**. Toda requisição HTTP passa por uma cadeia de middlewares, chamada de **Request Pipeline**.
   - **O que é um Middleware?**  
     É um componente que pode processar requisições e respostas. Cada middleware decide o que fazer: processar a requisição, passá-la para o próximo middleware ou interromper o pipeline retornando a resposta diretamente.
   - Exemplos de middlewares comuns:
     - `UseRouting()` – Identifica qual endpoint deve tratar a requisição
     - `UseAuthentication()` – Lida com autenticação (usuário está logado?)
     - `UseAuthorization()` – Verifica se o usuário tem permissão para acessar
     - `UseEndpoints()` / `MapControllers()` – Direciona a requisição ao controller adequado

> [!TIP]
> - Você pode criar seus próprios middlewares!  
> - Exemplos práticos incluem: logging de requisições/respostas, tratamento global de erros, limitação de requisições (rate limiting), cache de respostas, entre outros.
> - [No próximo tópico](#middlewares--pipeline-de-requisicao) eu explico melhor e com mais detalhes

3. Depois de passar pela cadeia de middlewares, ocorre o **processamento da requisição**:
   - O processamento é feito por um **controller**, que:
     - Usa o **routing** para identificar qual método (`Action`) deve responder
     - Recebe os dados da requisição (via `[FromBody]`, `[FromQuery]`, `[FromRoute]`, etc.)
     - Executa a lógica de negócio, geralmente por meio de serviços injetados via **Injeção de Dependência**
     - Retorna uma resposta (como `ActionResult`, `IActionResult`, JSON, etc.)
     
> [!TIP]
> - Para não deixar sem comentar...
> - `Routing` é o sistema que **mapeia as URLs das requisições HTTP** para os métodos (actions) dos controllers da aplicação/projeto. Ou seja, quando alguém acessa `/api/produtos/123`, o .NET precisa entender qual método deve ser executado, e o routing é quem faz isso.

## Middlewares (+ Pipeline de Requisição)

Normalmente você não vai dar muita atenção para middlewares em aplicações de desenvolvimento web. Mas para nível de conhecimento, vamos falar um pouco sobre os Middlewares.

Middlewares são o coração do pipeline de requisição HTTP no .NET. Tudo o que entra e sai da aplicação passa por eles.

Saber sobre middlewares te ajudará a:
    - Monitorar e regirar requisições globalmente.
    - Tratar exceções de forma centralizada.
    - Manipular headers e cookies.
    - Controlar autenticação/autorização.
    - Otimizar desemepenho con cache.
    - Interromper requisições antes de chegarem ao controller.
    
---

# Conteúdos Específicos

## Task

Esse tipo vem da **programação assíncrona** no .NET.

`Task` representa uma operação assíncrona que pode ainda estar em execução.

Quando você escreve um método com async, o retorno dele geralmente é um Task ou Task<T> (como Task<IActionResult>).

Dentro de um método com essa assinatura, significa que ele será **executado de forma assíncrona**, esperando um `await` em seu escopo.

---

## IActionResult

É uma interface que representa qualquer tipo de resposta HTTP em ASP.NET Core.

A partir dele podemos retornar:
- Ok() → 200 OK
- BadRequest() → 400 Bad Request
- NotFound() → 404 Not Found
- CreatedAtAction() → 201 Created

Portanto, o método com essa assinatura retorna uma resposta HTTP.

## Task + IActionResult

Task<IActionResult>: A união dos dois retornar uma **resposta HTTP de uma operação assíncrona**.


## Auto Mapper

### O que é?

O AutoMapper é uma biblioteca que:

- Mapeia objetos de um tipo para outro automaticamente, com base em **propriedades com nomes iguais**.
- Pode ser usado para **converter de DTO → Entidade e vice-versa**.
- Pode ser configurado com regras específicas via Profile.

### Como usar?
Para permitir seu uso são necessários alguns passos:

1. Instalação:
```
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```
2. Adicionar ao `Program.cs`:
```C#
builder.Services.AddAutoMapper(typeof(Program));
```
3. Criar um profile (como demonstrado no exemplo abaixo):
```C#
using AutoMapper;
using backend_wishlist.Models;

public class WishlistProfile : Profile
{
    public WishlistProfile()
    {
        CreateMap<WishList, WishList>(); // Mapeia a própria entidade para ela mesma
    }
}
```
4. Injetar no Controller:
```
private readonly IMapper _mapper;

public WishlistController(WishlistContext context, IMapper mapper)
{
    _context = context;
    _mapper = mapper;
}
```
5. Finalmente, usar no seu método do controller:
```C#
[HttpPut("{id}")]
public async Task<IActionResult> UpdateById(long id, [FromBody] WishList updatedItem)
{
    if (updatedItem == null || updatedItem.Id != id)
        return BadRequest("Dados inválidos.");

    var existingItem = await _context.WishLists.FindAsync(id);
    if (existingItem == null)
        return NotFound("Item não encontrado.");

    /* Ao invés de mapear um a um conforme abaixo...
    existingItem.Title = updatedItem.Title;
    existingItem.Description = updatedItem.Description;
    */

    /* ...você pode deixar o código mais limpo e fazer o mapeamento de forma prática e simples */
    _mapper.Map(updatedItem, existingItem); // Faz o mapeamento automático

    await _context.SaveChangesAsync();

    return Ok(existingItem);
}
```
