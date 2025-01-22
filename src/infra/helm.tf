resource "helm_release" "processador_video" {
  name       = "processador-video"
  namespace  = "default"  
  chart      = "./processamentovideo-chart"

  set{
    name = "environment"
    value = "prod"
  }     

  set{
    name = "aws.region"
    value = var.region
  }  

  set{
    name = "aws.QueueUrl"
    value = "https://sqs.${var.region}.amazonaws.com/${data.aws_caller_identity.current.account_id}"
  }

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
