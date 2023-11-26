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

resource "aws_ssm_parameter" "refresh_on_push" {
  name        = "/${var.meta.project}/${var.meta.env}/refresh_on_push"
  description = "Defines if instance refresh should execute when new image is pushed to ecr"
  type        = "String"
  value       = "true"
}
