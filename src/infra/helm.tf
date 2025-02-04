resource "helm_release" "processador_video" {
  name       = var.projectName
  namespace  = "default"
  chart      = "./processamentovideo-chart" 
  repository = "https://fluent.github.io/helm-charts"  # Repositório do Fluent Bit

  set {
    name  = "deployment.replicas"
    value = 1
  }

  set {
    name  = "environment"
    value = "prod"
  }

  set {
    name  = "aws.region"
    value = var.region
  }

  set {
    name  = "aws.accessKey"
    value = var.AWS_ACCESS_KEY_ID
  }

  set {
    name  = "aws.secret"
    value = var.AWS_SECRET_ACCESS_KEY
  }

  set {
    name  = "aws.token"
    value = var.AWS_SESSION_TOKEN
  }

  set {
    name  = "serviceAccount.role"
    value = var.labRole
  }

  set {
    name  = "serviceAccount.create"
    value = false
  }

  set {
    name  = "forceUpdate"
    value = "${timestamp()}"
  }

  set {
    name  = "ConnectionString"
    value = "Host=${local.rds_host_without_port};Port=5432;Pooling=true;Database=${var.dbName};User Id=${var.dbUsername};Password=${var.dbPassWord};"  
  }

  # Configuração do Fluent Bit (CloudWatch)
  set {
    name  = "fluentbit.enabled"
    value = "true"
  }

  set {
    name  = "fluentbit.cloudWatch.enabled"
    value = "true"
  }

  # Configuração do Log Group no CloudWatch
  set {
    name  = "fluentbit.cloudWatch.logGroupName"
    value = "processamento-video-api"  # Nome do Log Group no CloudWatch
  }

  # Região do CloudWatch
  set {
    name  = "fluentbit.cloudWatch.region"
    value = var.region  # Região AWS
  }

  # Prefixo para os logs no CloudWatch
  set {
    name  = "fluentbit.cloudWatch.logStreamPrefix"
    value = "eks-logs"  # Prefixo para os logs
  }

  # Criação automática do grupo de logs, caso não exista
  set {
    name  = "fluentbit.cloudWatch.autoCreateGroup"
    value = "true"  # Se o grupo de logs será criado automaticamente
  }

  # Criação automática dos streams dentro do grupo de logs
  set {
    name  = "fluentbit.cloudWatch.autoCreateStream"
    value = "true"  # Habilita a criação automática de streams se não existirem
  }

  # Configuração do tipo de coleta (exemplo de input, pode ser ajustado conforme necessidade)
  set {
    name  = "fluentbit.inputs"
    value = <<EOF
[INPUT]
  Name tail
  Path /var/log/containers/*.log
  Parser docker
  Tag kube.*
  Refresh_Interval 5
  Rotate_Wait 30s
EOF
  }

  depends_on = [
    aws_s3_bucket.bucket,
    aws_sqs_queue.converter_video_imagem_realizada,
    aws_sqs_queue.erro_converter_video_imagem,
    aws_sqs_queue.converter_video_imagem,
    aws_dynamodb_table.processamento_video_table,
    aws_db_instance.controle_pedido_db,
    aws_eks_node_group.eks-node,
    aws_eks_access_entry.eks-access-entry,
    aws_eks_access_policy_association.eks-access-policy,
    aws_eks_cluster.eks-cluster  ]
}
