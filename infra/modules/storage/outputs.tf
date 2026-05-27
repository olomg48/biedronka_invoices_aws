output "bucket_name" {
  value       = aws_s3_bucket.receipts.id
  description = "Bucket name"
}

output "bucket_arn" {
  value       = aws_s3_bucket.receipts.arn
  description = "Bucket ARN"
}

output "table_name" {
  value       = aws_dynamodb_table.receipts_table.name
  description = "DynamoDB table name"
}

output "table_arn" {
  value       = aws_dynamodb_table.receipts_table.arn
  description = "DynamoDB table arn"
}