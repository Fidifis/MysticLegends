resource "aws_ecr_repository" "this" {
  for_each             = var.repositories
  name                 = each.key
  image_tag_mutability = coalesce(each.value.immutable, var.default_config.immutable) ? "IMMUTABLE" : "MUTABLE"
  force_delete         = true

  image_scanning_configuration {
    scan_on_push = coalesce(each.value.scan, var.default_config.scan)
  }

  tags = local.module_tags
}

resource "aws_ecr_lifecycle_policy" "this" {
  # for_each = {
  #   for item in flatten([
  #     for repo_name, repo in var.repositories: [
  #       for i, policy in coalesce(repo.lifecycle, var.default_config.lifecycle) : {
  #         repo_name = repo_name
  #         number = i
  #         tagstatus = policy.tagstatus
  #         daysSincePush = policy.daysSincePush
  #       }
  #     ] if coalesce(repo.lifecycle, var.default_config.lifecycle) != null
  #   ]) : "${item.repo_name}.${item.number}" => item
  # }
  for_each = var.repositories
  repository = each.key

  policy = coalesce(each.value.lifecycle, var.default_config.lifecycle) == null ? null : jsonencode(
  {
    "rules": [ for i, policy in coalesce(each.value.lifecycle, var.default_config.lifecycle):
      {
        "rulePriority": i + 1,
        "description": "Expire old images",
        "selection": merge(
          {
            "tagStatus": policy.tagStatus,
            "countType": policy.type,
            "countNumber": policy.count
          },
          policy.tagPrefixList != null ? { "tagPrefixList": policy.tagPrefixList } : {},
          policy.type == "sinceImagePushed" ? { "countUnit": "days" } : {}
        ),
        "action": {
          "type": "expire"
        }
      }
    ]
  }
)

  depends_on = [ aws_ecr_repository.this ]
}
