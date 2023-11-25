terraform {
  required_version = "~> 1.6.4"

  required_providers {
    aws = {
      version = "~> 5.26.0"
    }
  }

  backend "s3" {
    bucket         = "mysticlegends-eu-west-1-terraform"
    key            = "mysticlegends-live-state.tfstate"
    dynamodb_table = "mysticlegends-terraform"
    region         = "eu-west-1"
    encrypt        = true
    profile        = "mysticlegends"
  }
}

provider "aws" {
  profile             = "mysticlegends"
  region              = local.meta.region
  allowed_account_ids = [local.meta.account]

  default_tags {
    tags = {
      Group       = "mysticlegends"
      Project     = local.meta.project
      IaC         = "terraform"
    }
  }
}
