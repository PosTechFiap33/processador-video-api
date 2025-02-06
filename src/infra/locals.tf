locals {
  rds_host_without_port = split(":", aws_db_instance.controle_pedido_db.endpoint)[0]
}