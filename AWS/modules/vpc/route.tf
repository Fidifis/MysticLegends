resource "aws_route_table_association" "public" {
  for_each       = var.public_subnets
  subnet_id      = aws_subnet.public[each.key].id
  route_table_id = aws_route_table.public.id
}

resource "aws_route_table_association" "nat" {
  for_each       = var.nat_subnets
  subnet_id      = aws_subnet.nat[each.key].id
  route_table_id = aws_route_table.nat[0].id
}

resource "aws_route_table" "nat" {
  count  = length(var.nat_subnets) > 0 ? 1 : 0
  vpc_id = aws_vpc.this.id

  route {
    cidr_block     = "0.0.0.0/0"
    nat_gateway_id = aws_nat_gateway.this[0].id
  }

  tags = merge(
    local.module_tags, {
      "Name" = "nat"
  })
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.this.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.this.id
  }

  route {
    ipv6_cidr_block = "::/0"
    gateway_id      = aws_internet_gateway.this.id
  }

  tags = merge(
    local.module_tags, {
      "Name" = "public"
  })
}
