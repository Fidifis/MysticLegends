variable "meta" {
  type = object({
    account = string
    project = string
    env     = string
    region  = string
  })
}

variable "vpc_id" {
  type = string
}

variable "prefix" {
  type = string
}

variable "open_ports" {
  type = map(object({
    ingress = set(number)
    description = optional(string)
    open_egress = optional(bool, true)
    security_groups = optional(set(string))
    protocol = optional(string, "-1")
  }))
}
