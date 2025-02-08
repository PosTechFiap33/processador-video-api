Feature: Processamento

  Scenario: Listar processamento por usuario com sucesso
    Given que tenha processsamentos cadastrados
    When for feita a requisição para a rota listagem de processamento
    Then devera ser retornado o status 200
    And devera ser retornado a lista de processamentos

  Scenario: Realizar o download do arquivo zip do processamento com sucesso
    Given que tenha processsamentos concluidos cadastrados
    When for feita a requisição para a rota download de processamento
    Then devera ser retornado o status 200
    And devera ser retornado o binario do arquivo zip