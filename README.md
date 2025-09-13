# BookFinder API

## Visão Geral do Projeto
BookFinder API é uma Web API construída com .NET 8 como parte de uma avaliação técnica para a vaga de Desenvolvedor Backend Júnior. O projeto se integra com a Open Library API para buscar informações sobre livros e autores, processa esses dados e os persiste em um banco de dados SQL Server, que é orquestrado via Docker Compose.

Além da funcionalidade principal, a API implementa um sistema completo de autenticação e autorização via JWT, CRUD para os dados locais, paginação e testes unitários.

## Features
- Integração com API de Terceiros: Busca de livros por autor e por ano de publicação na Open Library.
- Banco de Dados com Docker: Ambiente de desenvolvimento consistente e fácil de configurar com SQL Server rodando em um container Docker.
- Autenticação e Autorização: Sistema seguro de registro e login com JSON Web Tokens (JWT). Endpoints protegidos que exigem autenticação.
- CRUD Completo: Operações de Criar, Ler, Atualizar e Deletar para os autores salvos no banco de dados.
- Paginação: Os endpoints que retornam listas de dados são paginados para garantir a performance e escalabilidade da API.
- Testes Unitários: Testes para a camada de serviço (com Moq) e para a camada de controller (com EF Core In-Memory) garantindo a qualidade e a confiabilidade do código.
- Arquitetura Limpa: O projeto é organizado em camadas de responsabilidade (Api, Domain, Infrastructure), seguindo as melhores práticas de mercado.


## Tecnologias Utilizadas
- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server (via Docker)
- Docker Compose
- xUnit (para testes)
- Moq (para mocks em testes)
- BCrypt.Net-Next (para hashing de senhas)

## Pré-requisitos
Antes de começar, garanta que você tem as seguintes ferramentas instaladas:
- .NET 8 SDK
- Docker Desktop


## Como Configurar e Rodar
Siga estes passos para ter o projeto rodando localmente em poucos minutos.

1. Clone o repositório:
```bash
git clone https://github.com/josiasdev/BookFinderApi
cd BookFinderApi
```

2. Verifique os Arquivos de Configuração:
Os arquivos docker-compose.yml e BookFinder.Api/appsettings.json já estão pré-configurados. A senha padrão do banco de dados é BookFinder2509. Se desejar alterá-la, lembre-se de mudar nos dois arquivos:
- docker-compose.yml: na variável de ambiente SA_PASSWORD.
- BookFinder.Api/appsettings.json: na ConnectionString.

3. Inicie o Container do SQL Server:
Este comando irá baixar a imagem do SQL Server (apenas na primeira vez) e iniciar o container em segundo plano.

```bash
docker-compose up -d
```
Aguarde cerca de 1 minuto para que o serviço do SQL Server dentro do container seja totalmente inicializado.


4. Aplique as Migrations do Banco de Dados:
Este comando criará o banco de dados BookFinderDB e todas as tabelas necessárias dentro do container.
```bash
dotnet ef database update --startup-project BookFinder.Api
```

5. Execute a API:
```bash
dotnet run --project BookFinder.Api/BookFinder.Api.csproj
```

6. Acesse a Documentação e Teste:
A API estará rodando. Acesse a documentação interativa do Swagger para testar todos os endpoints:
- URL: https://localhost:PORTA/swagger (a porta geralmente é 7xxx, verifique o output do seu terminal).


## Estrutura do Projeto
O projeto utiliza uma arquitetura em camadas para separar as responsabilidades:
- BookFinder.Domain: Contém as entidades do banco de dados (Author, Book, User) e os DTOs (Data Transfer Objects), que definem os "contratos" de dados da aplicação.
- BookFinder.Infrastructure: Responsável pelo acesso a dados (DbContext) e pela comunicação com serviços externos (integração com a Open Library API, serviço de geração de token).
- BookFinder.Api: A camada de apresentação. Contém os Controllers, que expõem os endpoints HTTP, e o ponto de entrada da aplicação (Program.cs).
- BookFinder.Tests: Projeto de testes unitários.

## Endpoints da API

A seguir, uma lista dos principais endpoints disponíveis.

| Método HTTP | Endpoint                               | Descrição                                                              | Requer Autenticação? |
| :---------- | :------------------------------------- | :--------------------------------------------------------------------- | :------------------- |
| `POST`      | `Auth/register`                   | Registra um novo usuário.                                              | Não                  |
| `POST`      | `/Auth/login`                      | Autentica um usuário e retorna um token JWT.                           | Não                  |
| `GET`       | `/Books`                           | Lista todos os autores salvos (paginado).                              | **Sim** |
| `POST`      | `/Books/search-and-save/{authorName}`  | Busca um autor na Open Library e salva os dados.                       | **Sim** |
| `GET`       | `/Books/author/{id}`               | Busca um autor específico pelo seu ID.                                 | **Sim** |
| `PUT`       | `/Books/author/{id}`               | Atualiza o nome de um autor.                                           | **Sim** |
| `DELETE`    | `/Books/author/{id}`               | Deleta um autor e seus livros.                                         | **Sim** |
| `GET`       | `/Books/count`                     | Retorna a quantidade total de livros no banco.                         | **Sim** |
| `GET`       | `/{year}`               | Busca livros na Open Library por ano de publicação (paginado).         | **Sim** |



## Como Rodar os Testes
Para executar a suíte de testes unitários, navegue até a pasta raiz da solução e execute o comando:

```bash
dotnet test
```



## Link da API de Terceiros
- API: Open Library API
- Documentação: https://openlibrary.org/developers/api
