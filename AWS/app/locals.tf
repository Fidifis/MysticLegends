locals {
  common_prefix = "${var.meta.project}-${var.meta.env}"
  s3_prefix     = "${var.meta.project}-${var.meta.env}-${var.meta.region}"
}
