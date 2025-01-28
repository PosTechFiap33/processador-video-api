resource "helm_release" "processador_video" {
  name       = var.projectName
  namespace  = "default"
  chart      = "./processamentovideo-chart"

  set {
    name  = "environment"
    value = "prod"
  }

  set {
    name  = "aws.region"
    value = var.region
  }

  set {
    name  = "aws.QueueUrl"
    value = "https://sqs.${var.region}.amazonaws.com/${data.aws_caller_identity.current.account_id}"
  }

  set {
    name  = "serviceAccount.labRole"
    value = var.labRole
  }

  set {
    name  = "serviceAccount.principalRole"
    value = var.principalArn
  }

  set {
    name  = "serviceAccount.roles"
    value = join(",", concat(var.labRoles, [var.principalArn, "arn:aws:iam::874018567784:role/RoleForLambdaModLabRole", "arn:aws:iam::874018567784:role/AWSServiceRoleForCloudWatchEvents"]))
  }

  set {
    name  = "serviceAccount.create"
    value = true
  }

  set {
    name  = "forceUpdate"
    value = "${timestamp()}"
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

  set {
    name  = "fluentbit.cloudWatch.logGroupName"
    value = "processamento-video-api"  # Nome do Log Group do CloudWatch
  }

  set {
    name  = "fluentbit.cloudWatch.region"
    value = var.region  # Região AWS
  }

  set {
    name  = "fluentbit.cloudWatch.logStreamPrefix"
    value = "eks-logs"  # Prefixo para os logs
  }

  set {
    name  = "fluentbit.cloudWatch.autoCreateGroup"
    value = "true"  # Se o grupo de logs será criado automaticamente
  }

  depends_on = [
    aws_eks_node_group.eks-node,
    aws_eks_access_entry.eks-access-entry,
    aws_eks_access_policy_association.eks-access-policy,
    aws_eks_cluster.eks-cluster
  ]
}
