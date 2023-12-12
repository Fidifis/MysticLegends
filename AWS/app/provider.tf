terraform {
  required_version = "~> 1.6.4"

  backend "s3" { }

  required_providers {
    aws = {
      version = "~> 5.26"
    }
  }
}

provider "aws" {
  profile             = "mysticlegends"
  region              = var.meta.region
  allowed_account_ids = [var.meta.account]

  default_tags {
    tags = {
      Group       = "mysticlegends"
      Project     = var.meta.project
      Environment = var.meta.env
      IaC         = "terraform"
    }
  }
}
