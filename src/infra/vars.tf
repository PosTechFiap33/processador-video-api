variable "region" {
  default = "us-east-1"
}

variable "projectName" {
  default = "processador-video-cluster"
}

variable "labRole" {
  default = "arn:aws:iam::752307938219:role/LabRole"
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
  default = "arn:aws:iam::752307938219:role/voclabs"
}

variable "policyArn" {
  default = "arn:aws:eks::aws:cluster-access-policy/AmazonEKSClusterAdminPolicy"
}

variable "AWS_ACCESS_KEY" {}

variable "AWS_SECRET_KEY" {}

variable "k8s_host" {}

variable "k8s_ca_cert" {}

variable "k8s_token" {}