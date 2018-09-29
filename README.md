# DynamicSQL - para .NET

Projeto para realizar as operações do SQL de uma forma simples em seus projetos, com ele pode ser utilizado os comandos SQL como já conhecemos e pode ser utilizado uma entidade de dados com a mesma estrutura de campos de uma tabela de seu banco.

Inspirado no conceito de Micro ORM, ele não foi feito para ser um framework ORM e nem para facilitar os relacionamentos de um banco de dados, o conceito aqui é ter desempenho em aplicações que utilizam bando de dados para fazer as operações de select, insert, updade e delete sem precisa digitar vários comandos de SQL.

### Criar uma conexão com o Bando 

Para cria uma conexão com o banco de dado, devemos utilizar a classe de Conexão e informar a cadeia de informações que faz a conexão com o banco de dados, segue o exemplo

```C#
Conexao conexao = new Conexao("Server=NomeServido;Database=BancoDados;User Id=Usuario;Password=Senha;");
```

Nesse momento temos um objeto de conexão, ela ainda não foi aberta uma comunicação com o banco, ela será aberta quando iniciar as operações de SQL.

A partir desse momento você pode fazer a operações utilizando o método AbrirComandoSQL()

**OBS.:
Para fazer as operações no banco temos que seguir uma regra básica de sempre utilizar o using para abri um comando SQL**
