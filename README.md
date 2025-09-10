# BookFinder API

## Visão Geral do Projeto
Esta é uma Web API em .NET 8 que busca livros por autor utilizando a Open Library API. Ela processa os resultados e os armazena em um banco de dados SQL Server local. Este projeto foi desenvolvido como parte de uma avaliação técnica para a vaga de Desenvolvedor Backend Júnior.

## Como Configurar e Rodar
1.  Clone o repositório.
    ```bash
    git clone https://github.com/josiasdev/BookFinderApi
    ```
2.  Certifique-se de ter o .NET 8 SDK instalado.
3.  Configure a string de conexão no arquivo `BookFinder.Api/appsettings.json`. O projeto está configurado para usar o SQL Server LocalDB.
4.  Abra o terminal na pasta `BookFinder.Api` e execute os comandos de migração do banco:
    ```bash
    dotnet ef database update
    ```
5.  Execute o projeto:
    ```bash
    dotnet run --project BookFinder.Api/BookFinder.Api.csproj
    ```
6.  A API estará rodando em `https://localhost:PORTA` e a documentação Swagger estará disponível em `https://localhost:PORTA/swagger`.

## Link da API de Terceiros
- **API:** Open Library API
- **Documentação:** [https://openlibrary.org/developers/api](https://openlibrary.org/developers/api)
