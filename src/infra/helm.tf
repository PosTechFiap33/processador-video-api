resource "helm_release" "processador_video" {
  name       = var.projectName
  namespace  = "default"
  chart      = "./processamentovideo-chart" 
  timeout    = 1200
  version    = "1.1.1" 

  lifecycle {
    prevent_destroy = false
  }

  set {
    name  = "deployment.replicas"
    value = 1
  }
  
  set {
    name  = "environment"
    value = "prod"
  }

  set {
    name  = "SecretKey"
    value = var.SecretKey
  }

  set {
    name  = "aws.region"
    value = var.region
  }

  set {
    name  = "aws.serviceUrl"
    value = ""
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
    name  = "ConnectionString"
    value = "Host=${local.rds_host_without_port};Port=5432;Pooling=true;Database=${var.dbName};User Id=${var.dbUsername};Password=${var.dbPassWord};"  
  }

  depends_on = [
    aws_eks_cluster.eks-cluster  
 ]
}
