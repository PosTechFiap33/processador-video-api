variable "region" {
  default = "us-east-1"
}

variable "projectName" {
  default = "processador-video-cluster"
}

variable "labRole" {
  default = "arn:aws:iam::395510084268:role/LabRole"
}

variable "principalArn" {
  default = "arn:aws:iam::395510084268:role/voclabs"
}

variable "accessConfig" {
  default = "API_AND_CONFIG_MAP"
}

variable "nodeGroup" {
  default = "processador-videos-ng"
}

variable "eksInstanceType" {
  default = "t3.medium"
}

variable "policyArn" {
  default = "arn:aws:eks::aws:cluster-access-policy/AmazonEKSClusterAdminPolicy"
}

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

variable "rdsInstanceType" {
  default = "db.t3.micro"
}

variable "dbUsername" {
  description = "Nome de usuário do banco de dados"
  type        = string
}

variable "dbPassWord" {
  description = "Senha do banco de dados"
  type        = string
  sensitive   = true
}

variable "dbName" {
  description = "Nome do banco de dados"
  type        = string
}
