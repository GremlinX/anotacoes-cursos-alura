# Estudos sobre .NET (ASP.NET Core)

Anotações e conceitos importantes sobre o ecossistema .NET.

---

# Task

Esse tipo vem da **programação assíncrona** no .NET.

`Task` representa uma operação assíncrona que pode ainda estar em execução.

Quando você escreve um método com async, o retorno dele geralmente é um Task ou Task<T> (como Task<IActionResult>).

Dentro de um método com essa assinatura, significa que ele será **executado de forma assíncrona**, esperando um `await` em seu escopo.

---

# IActionResult

É uma interface que representa qualquer tipo de resposta HTTP em ASP.NET Core.

A partir dele podemos retornar:
- Ok() → 200 OK
- BadRequest() → 400 Bad Request
- NotFound() → 404 Not Found
- CreatedAtAction() → 201 Created

Portanto, o método com essa assinatura retorna uma resposta HTTP.

## Task + IActionResult

Task<IActionResult>: A união dos dois retornar uma **resposta HTTP de uma operação assíncrona**.

---

# Auto Mapper

## O que é?

O AutoMapper é uma biblioteca que:

- Mapeia objetos de um tipo para outro automaticamente, com base em **propriedades com nomes iguais**.
- Pode ser usado para **converter de DTO → Entidade e vice-versa**.
- Pode ser configurado com regras específicas via Profile.

## Como usar?
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
