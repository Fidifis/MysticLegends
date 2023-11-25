module "vpc" {
  source = "../modules/vpc"
  global = var.global

  supernet = "10.0.0.0"
  prefix = "16"
  public_subnets = {
    0 = "eu-west-1a"
    1 = "eu-west-1b"
    2 = "eu-west-1c"
  }
}
