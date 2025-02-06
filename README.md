# Repositorio para realizar o deploy da infra em K8S na aws

Este repositório contém a infraestrutura necessária para criar um cluster EKS (Elastic Kubernetes Service) na AWS utilizando Terraform. O processo de CI/CD é gerenciado pelo GitHub Actions, que automatiza a criação e a configuração da infraestrutura, bem como a implementação de aplicações no cluster usando Helm.

## Autores

- [Flávio Roberto Teixeira](https://github.com/FlavioRoberto)

## Estrutura do Repositório

```
.
├── .github/
│   └── workflows/
│       ├── deploy-eks.yml             # Configuração de deploy do Helm no cluster via GitHub Actions
│       ├── feature.yml                 # Configuração de validação dos arquivos TF via GitHub Actions
│       └── terraform.yml               # Valida, cria e executa o plano do Terraform na AWS
├── terraform/
│   ├── main.tf                         # Configuração principal do Terraform
│   ├── vars.tf                         # Definições de variáveis
│   ├── provider.tf                     # Configuração do provider do Terraform
│   ├── data.tf                         # Recupera informações de recursos da AWS (VPC e subnets)
│   ├── eks-cluster.tf                  # Cria o cluster do EKS
│   ├── eks-access-entry.tf             # Configurações do security group e ingress
│   ├── eks-access-policy.tf            # Política de acesso ao cluster
│   ├── eks-node.tf                     # Cria o node associado ao cluster
│   └── sg.tf                           # Cria o security group
└── k8s/
    └── controlepedidos-chart/
        ├── Chart.yaml                  # Definições do Chart
        ├── values.yaml                 # Valores do Chart
        └── templates/                  # Templates do Kubernetes
└── README.md                           # Documentação do repositório
```


## Pré-requisitos

- Conta na AWS com permissões para criar EKS e outros recursos necessários.
- Acesso ao Terraform Cloud ou Terraform.io para parametrização das variáveis.

## Configuração do Terraform

1. **Variáveis**: Edite o arquivo `variables.tf` para configurar as variáveis necessárias (como região, nome do cluster, etc.). Se o valor da variável for senssível a mesma deverá ser cadastrada no terraform.io.
2. **Estado Remoto**: Assegure que o Terraform esteja conectado ao Terraform.io para gerenciar o estado da infraestrutura.

## CI/CD com GitHub Actions

O fluxo de trabalho está definido em `.github/workflows`. Ele realiza as seguintes etapas:

1. **Feature**: Aciona a pipeline sempre que um push é feito na branch `feature` para validar os arquivos *.tf.
2. terraform.yaml: Aciona a pipeline sempre que um Pull Request é feito na branch `main` para criar os recursos de infra na AWS.
3. deploy-eks.yaml: Aciona a pipeline sempre que um push é feito na branch `main` para criar os recursos do K8S dentro do cluster (EKS).


## Executando o Projeto

1. **Clone o repositório**:
   git clone https://github.com/PosTechFiap33/processador-videos-infra-k8S.git
   cd processador-videos-infra-k8S

2. **Configurar credenciais**:
   - Configure suas credenciais da AWS e do Terraform Cloud e GitHub Secrets.

3. **Crie uma branch com préfixo feature**:
   - Realize as alterações necessárias na configuração do Terraform e nos charts do Helm.

4. **Faça o push**:
   - Aguarde a execução da pipe para validar os arquivos.

5. **Criar um Pull Request**:
   - Crie um pull request para a branch `main`. O GitHub Actions iniciará automaticamente o processo de criação da infra na AWS.

6. **Faça o merge na branch `main`**:
   - Realize o merge para que a pipe inicie o deploy do helm no EKS.


## Migrations Identity

dotnet ef migrations add {MigrationName} --project adapter/ProcessadorVideo.Identity -s adapter/ProcessadorVideo.Api -c IdentityContext --verbose

## Contribuições

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou pull requests.

