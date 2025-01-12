#!/bin/bash

awslocal sqs create-queue --queue-name converter-video-para-imagem

awslocal sqs receive-message --queue-url http://localhost:4566/000000000000/converter-video-para-imagem --max-number-of-messages 10 --visibility-timeout 30 --wait-time-seconds 0


#S3
##criar bucket
awslocal s3 mb s3://postech33-processamento-videos
##lista os buckets 
awslocal s3 ls
##listar objetos dentro do bucket
awslocal s3 ls s3://postech33-processamento-videos --recursive
## ex: awslocal s3 ls s3://postech33-processamento-videos/bdab9430-6160-4ae9-9695-5f99ea3e16f6/ --recursive


