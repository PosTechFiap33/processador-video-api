resource "helm_release" "processador_video" {
  name       = "processador-video"
  namespace  = "default"  # Ou substitua pelo seu namespace
  chart      = "./infra/processamentovideo-chart"
  values     = [
    "./infra/values-production.yaml"
  ]

  set {
    name  = "aws.accesKey"
    value = var.AWS_ACCESS_KEY
  }

  set {
    name  = "aws.secretKey"
    value = var.AWS_SECRET_KEY
  }

  set {
    name  = "api.configmap.database.value"
    value = var.DB_CONNECTION
  }

  set {
    name  = "forceUpdate"
    value = "${timestamp()}"
  }
}
