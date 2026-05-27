terraform {
    required_providers {
        aws = {
        source  = "hashicorp/aws"
        version = "~> 5.0"
        }
    }
}

provider "aws" {
    region     = "us-east-1"
    access_key = "mock" 
    secret_key = "mock"
    # localstack
    skip_credentials_validation = true
    skip_metadata_api_check     = true
    skip_requesting_account_id  = true
    s3_use_path_style           = true 
    endpoints {
        apigateway     = "http://raspberry.local:4566"
        cloudwatch     = "http://raspberry.local:4566"
        dynamodb       = "http://raspberry.local:4566"
        iam            = "http://raspberry.local:4566"
        lambda         = "http://raspberry.local:4566"
        s3             = "http://raspberry.local:4566"
        secretsmanager = "http://raspberry.local:4566"
        sqs            = "http://raspberry.local:4566"
    }
}
