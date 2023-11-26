module "ecr" {
  source = "../modules/ecr"
  meta = var.meta

  default_config = {
    immutable = false
    lifecycle = [
      {
        tagStatus = "untagged"
        daysSincePush = 1
      },
      {
        daysSincePush = 7
      }
    ]
  }
  repositories = {
    "mysticlegends-server": {}
    #"mysticlegends-postgres": {}
  }
}
