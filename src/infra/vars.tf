variable "region" {
  default = "us-east-1"
}

variable "projectName" {
  default = "processador-video-cluster"
}

variable "labRole" {
  default = "arn:aws:iam::874018567784:role/LabRole"
}

variable "accessConfig" {
  default = "API_AND_CONFIG_MAP"
}

variable "nodeGroup" {
  default = "processador-videos-ng"
}

variable "instanceType" {
  default = "t3.medium"
}

variable "principalArn" {
  default = "arn:aws:iam::874018567784:role/voclabs"
}

variable "policyArn" {
  default = "arn:aws:eks::aws:cluster-access-policy/AmazonEKSClusterAdminPolicy"
}

# variable "AWS_ACCESS_KEY_ID" {
#   description = "AWS Access Key ID"
#   type        = string
#   default     = "your-access-key-id"  # Valor padrão (não recomendado para credenciais sensíveis)
# }

# variable "AWS_SECRET_ACCESS_KEY" {
#   description = "AWS Secret Access Key"
#   type        = string
#   default     = "your-secret-access-key"  # Valor padrão (não recomendado para credenciais sensíveis)
# }

# variable "AWS_SESSION_TOKEN" {
#   description = "AWS Session Token"
#   type        = string
#   default     = "your-session-token"  # Valor padrão (não recomendado para credenciais sensíveis)
# }

variable "eksRoleName" {
    description = "Nome da role do eks"
    default = "eks-role"
}

variable "DynamoTableName"{
    description = "Nome da tabela do dynamo db"
    default = "ProcessamentoVideos"
}

variable "BucketName"{
    description = "Nome do bucket s3"
    default = "postech33-processamento-videos"
}
