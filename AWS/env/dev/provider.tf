terraform {
  required_version = "~> 1.6.4"

  required_providers {
    aws = {
      version = "~> 5.26.0"
    }
  }

  backend "s3" {
    bucket         = "isha-hub-dev-eu-west-1-terraform-state"
    key            = "isha-login.tfstate"
    dynamodb_table = "isha-hub-dev-terraform-state"
    region         = "eu-west-1"
    encrypt        = true
    profile        = "isha-hub"
  }
}

provider "aws" {
  profile             = "isha-hub"
  region              = local.meta.region
  allowed_account_ids = [local.meta.account]

  default_tags {
    tags = {
      Group       = "isha-hub"
      Project     = local.meta.project
      Environment = local.meta.env
      IaC         = "terraform"
    }
  }
}
