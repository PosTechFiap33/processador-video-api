resource "aws_sqs_queue" "converter_video_imagem" {
  name                        = "converter-video-para-imagem"  # Nome da fila
  delay_seconds               = 0                 # Tempo de atraso para a fila, em segundos
  maximum_message_size       = 262144            # Tamanho máximo da mensagem (em bytes)
  message_retention_seconds  = 345600            # Tempo máximo para reter as mensagens (em segundos, 4 dias)
  receive_message_wait_time_seconds = 20          # Tempo de espera para long polling (em segundos)
  visibility_timeout_seconds = 30                # Tempo para uma mensagem ficar oculta após ser lida, em segundos
}

output "converter_video_imagem_queue_url" {
  value = aws_sqs_queue.converter_video_imagem.url
}

output "converter_video_imagem_queue_arn" {
  value = aws_sqs_queue.converter_video_imagem.arn
}


resource "aws_sqs_queue" "erro_converter_video_imagem" {
  name                        = "erro-conversao-video-para-imagem"  # Nome da fila
  delay_seconds               = 0                 # Tempo de atraso para a fila, em segundos
  maximum_message_size       = 262144            # Tamanho máximo da mensagem (em bytes)
  message_retention_seconds  = 345600            # Tempo máximo para reter as mensagens (em segundos, 4 dias)
  receive_message_wait_time_seconds = 20          # Tempo de espera para long polling (em segundos)
  visibility_timeout_seconds = 30                # Tempo para uma mensagem ficar oculta após ser lida, em segundos
}

output "erro_converter_video_imagem_queue_url" {
  value = aws_sqs_queue.erro_converter_video_imagem.url
}

output "erro_converter_video_imagem_queue_arn" {
  value = aws_sqs_queue.erro_converter_video_imagem.arn
}




resource "aws_sqs_queue" "converter_video_imagem_realizada" {
  name                        = "conversao-video-para-imagem-realizada"  # Nome da fila
  delay_seconds               = 0                 # Tempo de atraso para a fila, em segundos
  maximum_message_size       = 262144            # Tamanho máximo da mensagem (em bytes)
  message_retention_seconds  = 345600            # Tempo máximo para reter as mensagens (em segundos, 4 dias)
  receive_message_wait_time_seconds = 20          # Tempo de espera para long polling (em segundos)
  visibility_timeout_seconds = 30                # Tempo para uma mensagem ficar oculta após ser lida, em segundos
}

output "converter_video_imagem_realizada_queue_url" {
  value = aws_sqs_queue.converter_video_imagem_realizada.url
}

output "converter_video_imagem_realizada_queue_arn" {
  value = aws_sqs_queue.converter_video_imagem_realizada.arn
}