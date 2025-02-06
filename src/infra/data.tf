data "aws_vpc" "vpc" {
  cidr_block = "172.31.0.0/16"
}

data "aws_subnets" "subnets" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.vpc.id]
  }
}

data "aws_subnet" "subnet" {
  for_each = toset(data.aws_subnets.subnets.ids)
  id       = each.value
}

# Obter o endpoint e o CA cert do cluster EKS
data "aws_eks_cluster" "eks_cluster" {
  name = aws_eks_cluster.eks-cluster.name
}

# Obter o token de autenticação do Kubernetes
data "aws_eks_cluster_auth" "eks_cluster_auth" {
  name = aws_eks_cluster.eks-cluster.name
}

data "aws_caller_identity" "current" {}

data "aws_region" "current" {}
