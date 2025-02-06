# resource "aws_iam_role" "eks_role" {
#   name = var.eksRoleName
  
#   assume_role_policy = jsonencode({
#     Version = "2012-10-17"
#     Statement = [
#       {
#         Action = "sts:AssumeRole"
#         Effect = "Allow"
#         Principal = {
#           Service = "ecs.amazonaws.com"  
#         }
#       }
#     ]
#   })
# }

# resource "aws_iam_policy" "eks_policy" {
#   name        = "eks-policy"
#   description = "Policy that allows read and write access to S3 and DynamoDB"
  
#   policy = jsonencode({
#     Version = "2012-10-17"
#     Statement = [
#       {
#         Action = [
#           "s3:GetObject",
#           "s3:PutObject",
#           "s3:ListBucket"
#         ]
#         Effect   = "Allow"
#         Resource = [
#           "arn:aws:s3:::${var.BucketName}",
#           "arn:aws:s3:::${var.BucketName}/*"        
#         ]
#       },
#       {
#         Action = [
#           "dynamodb:GetItem",
#           "dynamodb:PutItem",
#           "dynamodb:Scan",
#           "dynamodb:Query",
#           "dynamodb:UpdateItem"
#         ]
#         Effect   = "Allow"
#         Resource = "arn:aws:dynamodb:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:table/${var.DynamoTableName}"
#       }
#     ]
#   })
# }

# resource "aws_iam_role_policy_attachment" "attach_eks_policy" {
#   policy_arn = aws_iam_policy.eks_policy.arn
#   role       = aws_iam_role.eks_role.name
# }