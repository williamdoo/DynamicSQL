# DynamicSQL para .NET

Projeto para realizar as operações do SQL de uma forma simples em seus projetos, com ele pode ser utilizado os comandos SQL como já conhecemos e pode ser utilizado uma entidade de dados com a mesma estrutura de campos de uma tabela de seu banco.

Inspirado no conceito de Micro ORM, ele não foi feito para ser um framework ORM e nem para facilitar os relacionamentos do banco de dados nas aplicações, o conceito aqui é ter desempenho em aplicações que utilizam banco de dados para fazer as operações de select, insert, updade e delete sem precisa digitar vários comandos de SQL.

### Criar uma conexão com o Banco 

Para cria uma conexão com o banco de dado, devemos utilizar a classe Connection e informar a cadeia de informações que faz a conexão com o banco de dados, segue o exemplo

```C#
Connection conexao = new Connection("Server=NomeServidor;Database=BancoDados;User Id=Usuario;Password=Senha;");
```

Nesse momento temos um objeto de conexão, ela ainda não foi aberta uma comunicação com o banco, ela será aberta quando iniciar as operações de SQL.

A partir desse momento você pode fazer as operações utilizando o método OpenCommandSQL()

**OBS.:
Para fazer as operações no banco temos que seguir uma regra básica de sempre utilizar o using para abri um comando SQL**

### Select – Buscar informações no banco

Para fazer um comando select temos algumas possibilidades, mas vou mostrar pelo conceito que estamos acostumados, segue um exemplo de como realizar uma busca de clientes.

**Exemplo 1.0**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL())
{
    DataSet dt = comando.Select("select * from Clientes");
}
```

Foi utilizado o método Select que será retornado um DataSet para fazer a manipulação dos dados, esse é o jeito padrão onde você define como será seu select.

**Exemplo 2.0**
Esse exemplo podemos retorna os dados com uma entidade, segue o exemplo da entidade

```C#
[Table(Name = "CLIENTES")]
public class Cliente
{
    [Column(PrimaryKey = true, Increment = true)]
    public int ID_CLI { get; set; }
    public string NOME_CLI { get; set; }
    public string CPF_CLI { get; set; }
    public DateTime DATANASC_CLI { get; set; }
    public string TELEFONE_CLI { get; set; }
}
```

Veja que na classe foi criado um atributo **Table** com o Nome **CLIENTES**, esse nome é a Tabela do banco.
Também temos uma atribuição na propriedade ID_CLI que recebe dois atributos que vai definir que a propriedade é chave primária e ela é Incremento.

Seguem um exemplo de como realizar um select utilizando uma entidade

**Exemplo 2.1**

```C#
List<Cliente> clientes;
using (CommandSQL comando = conexao.OpenCommandSQL())
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
using (CommandSQL comando = conexao.OpenCommandSQL())
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

Temos outras duas formas mais simples de retornar os dados de uma tabela, nesse caso temos que utilizar uma entidade de dados

**Exemplo 3.0**

```C#
List<Cliente> cliente;
using (CommandSQL comando = conexao.OpenCommandSQL())
{
    cliente = comando.GetAll<Cliente>().ToList();
}
```

Foi utilizado o método GetAll para retornar um IList de todos os clientes.

**Exemplo 3.1**

```C#
List<Cliente> cliente;
using (CommandSQL comando = conexao.OpenCommandSQL())
{
    cliente = comando.Get<Cliente>("NOME_CLI like @NOME_CLI", new { NOME_CLI = "Vania" }).ToList();
}
```

Foi utilizado o método Get, esse método recebe dois parâmetros o primeiro é a cláusula where para filtrar as informações e o segundo para informar o valor que será aplicado na cláusula.

### Insert – Inserir dados no banco

Para inserir dados pode ser utiliza o conceito que estamos acostumados ou pela entidade.

**Exemplo 4.0**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL())
{
    int id = comando.Insert("insert into CLIENTES (NOME_CLI, CPF_CLI, DATANASC_CLI, TELEFONE_CLI) values ('Vania', '12345678900', '1992-01-14', '11987654321')");
}
```

Foi utilizado o método Insert com o comando SQL Insert no parâmetro, ele retorna o ID do registro, mas poderá vir 0 em caso de a tabela não tiver a coluna identity.

Temos uma outra forma de inserir dados de um modo simplificado.

**Exemplo 4.1**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL())
{
    int id = comando.Insert("CLIENTES", new { NOME_CLI = "Vania", CPF_CLI = "12345678900", DATANASC_CLI = "1992-01-14", TELEFONE_CLI = "11987654321" });
    comando.Commit();
}
```

Com o mesmo método Insert você só precisa passar o nome da tabela e os parâmetros e seus valores, retorna o ID do registro, mas poderá vir 0 em caso de a tabela não tiver a coluna identity.

Podemos utilizar a entidade para inserir os dados como mostra nesse exemplo

**Exemplo 4.2**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL())
{
    Cliente cliente = new Cliente()
    {
        NOME_CLI = "Vania",
        CPF_CLI = "12345678900",
        DATANASC_CLI = DateTime.Parse("1992-01-14"),
        TELEFONE_CLI ="11987654321"
    };

    int id = cliente.ID_CLI;
    int linhas = comando.Insert(cliente);
}
```

Com a entidade você cria um objeto e informa os dados necessários e passar no parâmetro do método Insert, se a tabela tiver um campo ID identity a entidade vai receber o valor na propriedade, quando você passar uma entidade no método Insert ele te retorna o número de linhas afetadas.

Você deve estar se perguntando “Quando eu inserir mais de um registro e um desses registro der algum erro como que eu faço dar um rollback?”. Muito simples, no método OpenCommandSQL é só passar um parâmetro de Begin Transaction como mostra nesse exemplo.

**Exemplo 4.3**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
{
    try
    {
        Cliente cliente = new Cliente()
        {
            NOME_CLI = "Vania",
            CPF_CLI = "12345678900",
            DATANASC_CLI = DateTime.Parse("1992-01-14"),
            TELEFONE_CLI ="11987654321"
        };

        int id = cliente.ID_CLI;
        int linhas = comando.Insert(cliente);
    
        comando.Commit();
    }
    catch
    {
        comando.Rollback();
    }
}
```

Como podemos ver no momento que abre um comando SQL você passa no parâmetro de frag begin transaction e no final você pode chamar o método Commit ou Rollback, para todos os exemplos segue o mesmo princípio.

### Update – Atualizar dados no banco

Para atualizar os dados pode ser utiliza o conceito que estamos acostumados ou pela entidade.

**Exemplo 5.0**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL())
{
    int linhasAlterada = comando.Update("update CLIENTES set TELEFONE_CLI = '119258741369' where ID_CLI = 123");
}
```

Foi utilizado o método Update passando o comando SQL Update, ele retorna os números de linhas alteradas.

**Exemplo 5.1**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
{
    int linhasAlterada = comando.Update("CLIENTES", new { TELEFONE_CLI = "119258741369" }, "ID_CLI = 123");
    comando.Commit();
}
```

Com o mesmo método Update você só precisa passar o nome da tabela e os parâmetros e seus valores, ele retorna os números de linhas alteradas.

**Exemplo 5.2**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
{
    Cliente fone = comando.Get<Cliente>("ID_CLI = @ID_CLI", new { ID_CLI = 123 }).FirstOrDefault();

    fone.TELEFONE_CLI = "119258741369";
    int linhasAlterada = comando.Update(fone);
    comando.Commit();
}
```

Podemos utilizar uma busca de dados e retornar em uma entidade e fazer as alterações das informações necessárias e passar no parâmetro do método Update para confirmar as alterações no banco de dados, ele retorna os números de linhas alteradas.

### Delete – Apagar os dados

Para apagar os dados pode ser utiliza o conceito que estamos acostumados ou pela entidade.

**Exemplo 6.0**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL())
{
    int linhasAlterada = comando.Delete("delete from CLIENTES where ID_CLI = 123");
}
```

Foi utilizado o método Delete passando o comando SQL Delete, ele retorna os números de linhas excludas.

**Exemplo 6.1**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
{
    int linhasAlterada = comando.Delete("CLIENTES", "ID_CLI = 123");
    comando.Commit();
}
```
Com o mesmo método Delete você só precisa passar o nome da tabela e a cláusula where para filtrar os dados que são para serem deletados, ele retorna os números de linhas excluidas.

**Exemplo 6.2**

```C#
using (CommandSQL comando = conexao.OpenCommandSQL(DynamicSQL.Flags.EnumBegin.Begin.BeginTransaction))
{
    Cliente fone = comando.Get<Cliente>("ID_CLI = @ID_CLI", new { ID_FONE = 123 }).FirstOrDefault();

    int linhasAlterada = comando.Delete(fone);
    comando.Commit();
}
```

Podemos utilizar uma busca de dados e retornar em uma entidade e passar no parâmetro do método Delete para confirmar a exclusão no banco de dados, ele retorna os números de linhas excluidas.
