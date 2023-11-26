resource "aws_ssm_parameter" "connectionstring" {
  name        = "/${var.meta.project}/${var.meta.env}/connectionstring"
  description = "Database connection string"
  type        = "SecureString"
  value       = "nothing_set"

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}
