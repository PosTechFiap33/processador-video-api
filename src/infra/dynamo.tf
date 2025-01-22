resource "aws_dynamodb_table" "processamento_video_table" {
  name           = "ProcessamentoVideos"   # Nome da tabela
  billing_mode   = "PROVISIONED"           # Pode ser "PROVISIONED" ou "PAY_PER_REQUEST"
  hash_key       = "Id"                    # Chave primária
  range_key      = "SortKey"               # Chave de ordenação (opcional)
  read_capacity  = 5                       # Capacidade de leitura provisionada
  write_capacity = 5                       # Capacidade de gravação provisionada

  # Definindo os atributos da tabela
  attribute {
    name = "Id"
    type = "S"  # Tipo de dado da chave hash (S = String, N = Número, B = Binário)
  }

  attribute {
    name = "SortKey"
    type = "S"  # Tipo de dado da chave de ordenação
  }

  attribute {
    name = "usuarioId"
    type = "S"  # Tipo de dado para o índice
  }

  # Índice Secundário Global (GSI) baseado no atributo usuarioId
  global_secondary_index {
    name            = "UsuarioId"            # Nome do índice secundário global
    hash_key        = "usuarioId"            # Chave hash do índice
    projection_type = "ALL"                  # Todos os atributos da tabela serão projetados para o índice
    read_capacity   = 5                       # Capacidade de leitura provisionada para o índice
    write_capacity  = 5                       # Capacidade de gravação provisionada para o índice
  }

  # Tags (opcional)
  tags = {
    Name        = "Processamento Video DynamoDB Table"
    Environment = "Production"
  }
}
