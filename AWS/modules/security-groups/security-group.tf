resource "aws_security_group" "this" {
  for_each    = var.open_ports
  name        = "${var.prefix}-${each.key}"
  description = each.value.description
  vpc_id      = var.vpc_id

  dynamic "ingress" {
    for_each = each.value.ingress
    content {
      from_port        = ingress.value
      to_port          = ingress.value
      protocol         = each.value.protocol
      cidr_blocks      = each.value.security_groups == null ? ["0.0.0.0/0"] : null
      ipv6_cidr_blocks = each.value.security_groups == null ? ["::/0"] : null
      security_groups  = each.value.security_groups
    }
  }
  dynamic "egress" {
    for_each = [each.value.open_egress ? 1 : 0]
    content {
      from_port        = 0
      to_port          = 0
      protocol         = "-1"
      cidr_blocks      = ["0.0.0.0/0"]
      ipv6_cidr_blocks = ["::/0"]
    }
  }
}
