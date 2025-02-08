Feature: Usuario

  Scenario: Cadastrar um usuario com sucesso
    Given que eu informe o nome de identificacao "kenobi"
    And o email "teste@teste.com"
    And que eu informe a senha  "hellothere"
    When for feita a requisição para a rota de cadastro
    Then devera ser retornado o status 201
    And o id do usuario deve ser válido

  Scenario: Cadastrar um usuario já existente
    Given que eu informe o nome de identificacao "vader"
    And o email "existente@teste.com"
    And que eu informe a senha  "jaexiste"
    When for feita a requisição para a rota de cadastro
    Then devera ser retornado o status 400
    And devera ser retornado a mensagem de erro "Ja existe um usuario com esse nome cadastrado!"

  Scenario: Autenticar o usuario cadastrado com sucesso
    Given que eu informe o nome de identificacao "vader"
    And que eu informe a senha  "darkside"
    When for realizada uma autenticacao com o usuario e senha informados
    Then devera ser retornado o status 200
    And devera ser retornado o token

  Scenario: Autenticar o usuario sem informar o nome de identificacao
    Given que eu informe a senha  "darkside"
    When for realizada uma autenticacao com o usuario e senha informados
    Then devera ser retornado o status 400
    And devera ser retornado a mensagem de erro "Nome de identificação não informado!"

  Scenario: Autenticar o usuario sem informar a senha
    Given que eu informe o nome de identificacao "vader"
    When for realizada uma autenticacao com o usuario e senha informados
    Then devera ser retornado o status 400
    And devera ser retornado a mensagem de erro "Senha não informada!"

  Scenario: Autenticar um usuario inexistente
    Given que eu informe o nome de identificacao "yoda"
    And que eu informe a senha  "master"
    When for realizada uma autenticacao com o usuario e senha informados
    Then devera ser retornado o status 400
    And devera ser retornado a mensagem de erro "usuario yoda não encontrado!"

  Scenario: Autenticar o usuario com senha incorreta
    Given que eu informe o nome de identificacao "vader"
    And que eu informe a senha  "lightside"
    When for realizada uma autenticacao com o usuario e senha informados
    Then devera ser retornado o status 400
    And devera ser retornado a mensagem de erro "Autenticacao inválida para o usuario vader!"
