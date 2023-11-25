variable "global" {
  type = object({
    account = string
    project = string
    env     = string
    region  = string
  })
}

variable "supernet" {
  type = string
  default = "10.0.0.0"
}

variable "prefix" {
  type = number
  default = 16
}

variable "public_subnets" {
  type = map(string)
  default = {}
}

variable "nat_subnets" {
  type = map(string)
  default = {}
}

variable "private_subnets" {
  type = map(string)
  default = {}
}

variable "nat_gateway_subnet" {
  type    = number
  default = 0
}
