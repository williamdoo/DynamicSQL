# DynamicSQL para .NET

Projeto para realizar as operações do SQL de uma forma simples em seus projetos, com ele pode ser utilizado os comandos SQL como já conhecemos e pode ser utilizado uma entidade de dados com a mesma estrutura de campos de uma tabela de seu banco.

Inspirado no conceito de Micro ORM, ele não foi feito para ser um framework ORM e nem para facilitar os relacionamentos do banco de dados nas aplicações, o conceito aqui é ter desempenho em aplicações que utilizam bando de dados para fazer as operações de select, insert, updade e delete sem precisa digitar vários comandos de SQL.

### Criar uma conexão com o Banco 

Para cria uma conexão com o banco de dado, devemos utilizar a classe Conexão e informar a cadeia de informações que faz a conexão com o banco de dados, segue o exemplo

```C#
Conexao conexao = new Conexao("Server=NomeServidor;Database=BancoDados;User Id=Usuario;Password=Senha;");
```

Nesse momento temos um objeto de conexão, ela ainda não foi aberta uma comunicação com o banco, ela será aberta quando iniciar as operações de SQL.

A partir desse momento você pode fazer as operações utilizando o método AbrirComandoSQL()

**OBS.:
Para fazer as operações no banco temos que seguir uma regra básica de sempre utilizar o using para abri um comando SQL**

### Select – Buscar informações no banco

Para fazer um comando select temos algumas possibilidades, mas vou mostrar pelo conceito que estamos acostumados, segue um exemplo de como realizar uma busca de clientes.

**Exemplo 1.0**

```C#
using (ComandoSQL comando = conexao.AbrirComandoSQL())
{
    DataSet dt = comando.Select("select * from Clientes");
}
```

Foi utilizado o método Select que será retornado um DataSet para fazer a manipulação dos dados, esse é o jeito padrão onde você define como será seu select.

**Exemplo 2.0**
Esse exemplo podemos retorna os dados com uma entidade, segue o exemplo da entidade

```C#
[TabelaDB(Nome = "CLIENTES")]
public class Cliente
{
    [CampoDB(ChavePrimaria = true, Incremento = true)]
    public int ID_CLI { get; set; }
    public string NOME_CLI { get; set; }
    public string CPF_CLI { get; set; }
    public DateTime DATANASC_CLI { get; set; }
    public string TELEFONE_CLI { get; set; }
}
```

Veja que na classe foi criado um atributo **TabelaDB** com o Nome **CLIENTES**, esse nome é a Tabela do banco.
Também temos uma atribuição na propriedade ID_CLI que recebe dois atributos que vai definir que a propriedade é chave primária e ela é Incremento.

Seguem um exemplo de como realizar um select utilizando uma entidade

**Exemplo 2.1**

```C#
List<Cliente> clientes;
using (ComandoSQL comando = conexao.AbrirComandoSQL())
{
    clientes = comando.Select<Cliente>("select * from Clientes").ToList();
}
```

Foi utilizado o método Select para retornar um IList de Clientes para fazer a manipulação dos dados, alterar as informações ou apagar no banco de dados, veja que o foi utilizado o comando Select no parâmetro.

Você não precisa necessariamente utilizar uma entidade de dado caso não for fazer alterações, pode criar uma classe de apoio com os mesmos nomes de campo da tabela para criar consulta elaborada com relacionamento de outras tabelas.

**Exemplo 2.2**

Seguem um exemplo de uma Classe de apoio

```C#
public class ConsultaComprasCliente
{
    public int ID_CLI { get; set; }
    public string NOME_CLI { get; set; }
    public int QTDE_COMPRA { get; set; }
}

List<ConsultaComprasCliente> consulta;
using (ComandoSQL comando = conexao.AbrirComandoSQL())
{
    consulta = comando.Select<ConsultaComprasCliente>("select 
	    ID_CLI,
	    NOME_CLI
	    COUNT(ID_VDA) as QTDE_COMPRA
    from CLIENTES C
    join VENDAS V on C.ID_CLI = V.ID_CLI
    GROUP BY  ID_CLI, NOME_CLI").ToList();
}
```

Aqui será retornado um IList com as informações de quantidade de compra que o cliente fez.
