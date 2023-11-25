resource "aws_vpc" "this" {
  cidr_block                       = "${var.supernet}/${var.prefix}"
  assign_generated_ipv6_cidr_block = true
  enable_dns_support               = true
  enable_dns_hostnames             = true

  tags = merge(
    local.module_tags, {
      Name = "${var.global.project}"
  })
}

resource "aws_subnet" "public" {
  for_each                        = var.public_subnets
  availability_zone               = each.value
  vpc_id                          = aws_vpc.this.id
  cidr_block                      = cidrsubnet(cidrsubnet(aws_vpc.this.cidr_block, 2, 0), 6, each.key)
  ipv6_cidr_block                 = cidrsubnet(aws_vpc.this.ipv6_cidr_block, 8, each.key)
  map_public_ip_on_launch         = true
  assign_ipv6_address_on_creation = true

  tags = merge(
    local.module_tags, {
      "Name" = "${var.global.project}-public-${each.value}-${each.key}"
  })
}

resource "aws_subnet" "nat" {
  for_each                = var.nat_subnets
  availability_zone       = each.value
  vpc_id                  = aws_vpc.this.id
  cidr_block              = cidrsubnet(cidrsubnet(aws_vpc.this.cidr_block, 2, 1), 6, each.key)
  map_public_ip_on_launch = false

  tags = merge(
    local.module_tags, {
      "Name" = "${var.global.project}-nat-${each.value}-${each.key}"
  })
}

resource "aws_subnet" "private" {
  for_each                = var.private_subnets
  availability_zone       = each.value
  vpc_id                  = aws_vpc.this.id
  cidr_block              = cidrsubnet(cidrsubnet(aws_vpc.this.cidr_block, 2, 2), 6, each.key)
  map_public_ip_on_launch = false

  tags = merge(
    local.module_tags, {
      "Name" = "${var.global.project}-private-${each.value}-${each.key}"
  })
}
