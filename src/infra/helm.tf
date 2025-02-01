resource "helm_release" "processador_video" {
  name       = var.projectName
  namespace  = "default"
  chart      = "./processamentovideo-chart"  # Mantém o caminho para o chart local, se necessário
  repository = "https://fluent.github.io/helm-charts"  # Repositório do Fluent Bit

  # Variáveis de configuração do processador de vídeo
  set {
    name  = "environment"
    value = "prod"
  }

  set {
    name  = "aws.region"
    value = var.region
  }

  set {
    name  = "serviceAccount.role"
    value = var.labRole
  }

  set {
    name  = "serviceAccount.create"
    value = true
  }

  set {
    name  = "forceUpdate"
    value = "${timestamp()}"
  }

  set {
    name  = "ConnectionString"
    value = "Host=${aws_db_instance.controle_pedido_db.endpoint};Port=5432;Pooling=true;Database=${var.dbUsername};User Id=${var.dbUsername};Password=${var.dbPassWord};"  
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
    aws_db_instance.controle_pedido_db,
    aws_eks_node_group.eks-node,
    aws_eks_access_entry.eks-access-entry,
    aws_eks_access_policy_association.eks-access-policy,
    aws_eks_cluster.eks-cluster
  ]
}
