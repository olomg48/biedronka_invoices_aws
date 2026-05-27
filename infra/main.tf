module "storage" {
  source = "./modules/storage"
  bucket_name = var.bucket_name
  table_name  = var.table_name
}