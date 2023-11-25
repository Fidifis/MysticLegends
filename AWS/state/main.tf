module "state" {
  source = "../modules/tf-state"
  bucket_name = "${local.meta.project}-${local.meta.region}-terraform"
  table_name = "${local.meta.project}-terraform"
}
