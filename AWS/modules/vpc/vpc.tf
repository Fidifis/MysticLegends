resource "aws_vpc" "this" {
  cidr_block                       = "${var.supernet}/${var.prefix}"
  assign_generated_ipv6_cidr_block = true
  enable_dns_support               = true
  enable_dns_hostnames             = true

  tags = merge(
    local.module_tags, {
      Name = "${var.meta.project}"
  })
}
