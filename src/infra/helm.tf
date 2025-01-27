resource "helm_release" "processador_video" {
  name       = var.projectName
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

  set{
    name = "serviceAccount.roleArn"
    value = var.labRole
  }

  set{
    name = "serviceAccount.create"
    value = true
  }

  set {
    name  = "forceUpdate"
    value = "${timestamp()}"
  }

  depends_on = [
#    aws_iam_policy.eks_policy,
#    aws_iam_role.eks_role,
    aws_eks_node_group.eks-node,
    aws_eks_access_entry.eks-access-entry,
    aws_eks_access_policy_association.eks-access-policy,
    aws_eks_cluster.eks-cluster 
  ]
  
}
