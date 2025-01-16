#!/bin/bash
#converter-video-para-imagem
#conversao-video-para-imagem-realizada
#erro-conversao-video-para-imagem

awslocal sqs create-queue --queue-name converter-video-para-imagem
awslocal sqs create-queue --queue-name conversao-video-para-imagem-realizada
awslocal sqs create-queue --queue-name erro-conversao-video-para-imagem

awslocal sqs receive-message --queue-url http://localhost:4566/000000000000/converter-video-para-imagem --max-number-of-messages 10 --visibility-timeout 30 --wait-time-seconds 0

awslocal sqs delete-queue --queue-url http://localhost:4566/000000000000/converter-video-para-imagem


#S3
##criar bucket
awslocal s3 mb s3://postech33-processamento-videos
##lista os buckets 
awslocal s3 ls
##listar objetos dentro do bucket
awslocal s3 ls s3://postech33-processamento-videos --recursive
## ex: awslocal s3 ls s3://postech33-processamento-videos/bdab9430-6160-4ae9-9695-5f99ea3e16f6/ --recursive


##dynamodb
aws dynamodb delete-table --table-name ProcessamentoVideos

awslocal dynamodb create-table \
    --table-name ProcessamentoVideos \
    --attribute-definitions \
        AttributeName=Id,AttributeType=S \
        AttributeName=Status,AttributeType=S \
    --key-schema \
        AttributeName=Id,KeyType=HASH \
        AttributeName=Status,KeyType=RANGE \
    --provisioned-throughput \
        ReadCapacityUnits=5,WriteCapacityUnits=5

awslocal dynamodb scan --table-name ProcessamentoVideos



