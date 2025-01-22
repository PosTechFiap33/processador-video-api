terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    },
    helm = {
      source  = "hashicorp/helm"
      version = "2.7.1"
    }
  }
  required_version = ">= 1.1.0"

  cloud {
    organization = "PosTechFiap33"

    workspaces {
      name = "ProcessamentoVideoApi"
    }
  }
}

provider "aws" {
  region = var.region
}

provider "helm" {
  kubernetes {
    host                   = "https://${var.k8s_host}"
    cluster_ca_certificate = base64decode(var.k8s_ca_cert)
    token                  = var.k8s_token
  }
}