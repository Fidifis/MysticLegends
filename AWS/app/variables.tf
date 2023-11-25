variable "global" {
  type = object({
    account = string
    project = string
    env     = string
    region  = string
  })
}
