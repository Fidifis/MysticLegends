module "ecr" {
  source = "../modules/ecr"
  meta = var.meta

  default_config = {
    immutable = false
    lifecycle = [
      {
        tagStatus = "untagged"
        type = "sinceImagePushed"
        count = 1
      },
      {
        tagStatus = "tagged"
        tagPrefixList = ["latest"]
        type = "imageCountMoreThan"
        count = 1
      },
      {
        tagStatus = "any"
        type = "sinceImagePushed"
        count = 7
      },
    ]
  }
  repositories = {
    "mysticlegends-server": {}
    #"mysticlegends-postgres": {}
  }
}
