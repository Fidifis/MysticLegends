locals {
  common_prefix = "${var.global.project}-${var.global.env}"
  s3_prefix     = "${var.global.project}-${var.global.env}-${var.global.region}"
}
