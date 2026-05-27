output "bucket_name" {
  value       = module.storage.bucket_name
  description = "Bucket name"
}

output "bucket_arn" {
  value       = module.storage.bucket_arn
  description = "Bucket ARN"
}

output "table_name" {
  value       = module.storage.table_name
  description = "DynamoDB table name"
}

output "table_arn" {
  value       = module.storage.table_arn
  description = "DynamoDB table arn"
}