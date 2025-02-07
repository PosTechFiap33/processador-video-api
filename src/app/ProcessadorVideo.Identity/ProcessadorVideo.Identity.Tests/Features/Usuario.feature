Feature: Usuario

  Scenario: Cadastrar um usuario com sucesso
    Given que eu informe o nome de identificacao "kenobi"
    And o email "teste@teste.com"
    And a senha "hellothere"
    When for feita a requisição para a rota de cadastro
    Then devera ser retornado o status 201
    And o id do usuario deve ser válido

  Scenario: Autenticar o usuario cadastrado com sucesso
    Given que eu informe o nome de identificacao "vader"
    And a senha "darkside"
    When for realizada uma autenticacao com o usuario e senha informados
    Then devera ser retornado o status 200
    And devera ser retornado o token
