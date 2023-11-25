data "aws_availability_zones" "this" {}

module "vpc" {
  source = "../modules/vpc"
  meta = var.meta

  supernet = "10.0.0.0"
  prefix = "16"
  public_subnets = {
    for i, zone in data.aws_availability_zones.this.names: i => zone
  }
}
