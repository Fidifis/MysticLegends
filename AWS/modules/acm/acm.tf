resource "aws_acm_certificate" "this" {
  for_each          = var.domains
  domain_name       = each.key
  validation_method = "DNS"

  tags = local.module_tags

  lifecycle {
    create_before_destroy = true
  }
}
