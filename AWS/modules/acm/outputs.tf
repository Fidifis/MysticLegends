output "arns" {
  value = { for cert in aws_acm_certificate.this : cert.domain_name => cert.arn }
}
