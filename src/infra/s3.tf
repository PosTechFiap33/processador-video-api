resource "aws_s3_bucket" "bucket" {
  bucket = "postech33-processamento-videos"  # Nome único do bucket

  # Habilitando versionamento
  versioning {
    enabled = true
  }

  # Configurando tags (opcional)
  tags = {
    Name        = "Bucket que contém os vídeos processados"
    Environment = "Production"
  }

  # Política do Bucket (Permite acesso público apenas para leitura e gravação nos objetos do bucket)
  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:GetObject",
        "s3:PutObject"
      ],
      "Resource": "arn:aws:s3:::postech33-processamento-videos/*"
    }
  ]
}
POLICY

  # Configuração CORS (permite compartilhamento de recursos)
  cors_rule {
    allowed_headers = ["*"]
    allowed_methods = ["GET", "POST"]
    allowed_origins = ["*"]
    expose_headers  = ["x-amz-request-id"]
  }
}

# Configuração de bloqueio de acesso público para o bucket S3
resource "aws_s3_bucket_public_access_block" "block_public_access" {
  bucket = aws_s3_bucket.bucket.id

  block_public_acls      = true
  block_public_policy    = true
  ignore_public_acls     = true
  restrict_public_buckets = true
}
