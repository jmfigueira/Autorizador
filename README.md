# Code Challenge - Autorizador
Teste de software.

# Resumo
Tendo em vista todas as oportuidades que o mercado oferece, eu viso trabalhar em uma empresa onde o planejamento e a tecnologia realmente importam.
Também é importante levar em consideração um lugar onde as pessoas se importem com o produto e sua engenharia.

# Como foi feito?

Este é um pequeno Console Application feito em .NET CORE que usa a documentação fornecida para resolver o fluxo de criação de conta e controle de transações de um cliente, onde o programa deve lidar com dois tipos de operações, decidindo qual delas executar de acordo com a linha que estiver sendo processada.

Esta solução foi feita com a ideia de ser o mais simples possível, abordando apenas o necessário para para construir o seu contexto. Então não foi feita a utilização de muitas frameworks externas. 

O código foi escrito 100% em C# (.NET CORE 5.0).

A arquitetura foi construída como um microsserviço utilizando DDD, com exemplos de injeção de dependência e cobertura de teste usando a framework NUNIT com instrumentação de dados.

Como o programa não deve depender de nenhum banco de dados externo e o estado interno da aplicação deve ser gerenciado em memória, explicitamente, por alguma estrutura e o estado da aplicação deve estar vazio sempre que a aplicação for inicializada, então, foi utilziado o MemoryCache do .NET levando em consideração a construção do seu despejo (Dispose) para não segurar memória em sistemas que não fazem o gerenciamento de estado automaticamente.

Na questão, entrada de dados, para realizar a conversão do texto (input - json) para trabalhar com objetos em memória, foi utilizado a biblioteca Newtonsoft.

# Setup
A aplicação utiliza apenas .NET CORE 5.0

Building e execução dos testes:

Será necessário o VSCODE ou o Visual Studio com suporte ao .NET Core 5.0

Na raiz do projeto (execução do projeto):
  
  - Entrar na pasta .\authorizer\
  - Executar os comando:
    - Dotnet restore
    - Dotnet build
    - Dotnet run

Essas tarefas irão criar todas as dlls importantes para conseguir executar a aplicação ou rodar os testes.

Na raiz do projeto (execução dos testes):

  - Entrar na pasta .\authorizer_tests\
  - Executar os comando:
    - Dotnet restore
    - Dotnet build
    - Dotnet test

Se necessário executar os testes de instrução com outros parâmetros. Poderá ser aproveitado as variáveis simuladas desta construção, uma vez que essas variáveis contam com Mock e já estão configuradas de acordo com o cenário desejado.

#### NOTA - A aplicação está apta a rodar em sistemas Unix, Windows ou Mac.
