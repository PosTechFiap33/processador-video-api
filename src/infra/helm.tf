resource "helm_release" "processador_video" {
  name       = "processador-video"
  namespace  = "default"  # Ou substitua pelo seu namespace
  chart      = "./infra/processamentovideo-chart"
  values     = [
    "./infra/values-production.yaml"
  ]

  set {
    name  = "aws.accesKey"
    value = var.AWS_ACCESS_KEY_ID
  }

  set {
    name  = "aws.secretKey"
    value = var.AWS_SECRET_ACCESS_KEY
  }

  set {
    name  = "aws.sessionToken"   # Passando o session token
    value = var.AWS_SESSION_TOKEN
  }

  set {
    name  = "forceUpdate"
    value = "${timestamp()}"
  }
  
}
