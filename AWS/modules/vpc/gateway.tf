resource "aws_internet_gateway" "this" {
  vpc_id = aws_vpc.this.id

  tags = merge(
    local.module_tags,
    {
      Name = "internet-gateway"
  })
}

resource "aws_eip" "nat" {
  count = length(var.nat_subnets) > 0 ? 1 : 0

  tags = merge(
    local.module_tags,
    {
      Name = "nat-gateway-ip"
  })
}

resource "aws_nat_gateway" "this" {
  count             = length(var.nat_subnets) > 0 ? 1 : 0
  connectivity_type = "public"
  allocation_id     = aws_eip.nat[0].allocation_id
  subnet_id         = aws_subnet.public[var.nat_gateway_subnet].id

  tags = merge(
    local.module_tags, {
      Name = "nat-gateway"
  })
}
