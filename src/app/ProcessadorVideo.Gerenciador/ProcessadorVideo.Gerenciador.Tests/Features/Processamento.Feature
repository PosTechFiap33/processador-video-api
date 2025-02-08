Feature: Processamento

  Scenario: Cadastrar processamento sem informar o arquivo de video
    Given que eu nao informe um arquivo de video valido
    When for feita a requisição para a rota de cadastro de processamento
    Then devera ser retornado o status 400
    And devera ser exibida uma mensagem de erro "Informe um video para ser processado."

  Scenario: Cadastrar processamento com sucesso
    Given que eu informe um arquivo de video valido
    When for feita a requisição para a rota de cadastro de processamento
    Then devera ser retornado o status 201
    And devera ser retornado o id do processamento

  Scenario: Listar processamento por usuario com sucesso
    Given que tenha processsamentos cadastrados com id "93277f0f-087b-4aaa-b7bf-e649c50676b7"
    When for feita a requisição para a rota listagem de processamento
    Then devera ser retornado o status 200
    And devera ser retornado a lista de processamentos

  Scenario: Realizar o download do arquivo zip do processamento com sucesso
    Given que tenha processsamentos concluidos cadastrados
    When for feita a requisição para a rota download de processamento
    Then devera ser retornado o status 200
    And devera ser retornado o binario do arquivo zip

  Scenario: Realizar o download do arquivo zip sem informar o id do processamento
    Given que eu nao tenha processsamentos cadastrados
    When for feita a requisição para a rota download de processamento
    Then devera ser retornado o status 400
    And devera ser exibida uma mensagem de erro "Id do processamento não foi informado!"

  Scenario: Realizar o download do arquivo zip informando um id de processamento inexistente
    Given que eu informe um id de processamento inexistente "93277e0f-087b-4aaa-b7bf-e649c50676b6"
    When for feita a requisição para a rota download de processamento
    Then devera ser retornado o status 404
    And devera ser exibida uma mensagem de erro "Não foi encontrado um processamento com id 93277e0f-087b-4aaa-b7bf-e649c50676b6"

  Scenario: Realizar o download do arquivo zip inexistente
    Given que tenha processsamentos cadastrados com id "93277e0f-087b-4aaa-b7bf-e649c50676b7"
    When for feita a requisição para a rota download de processamento
    Then devera ser retornado o status 404
    And devera ser exibida uma mensagem de erro "Não foi encontrado um processamento com id 93277e0f-087b-4aaa-b7bf-e649c50676b7"