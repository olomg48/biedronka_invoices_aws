resource "aws_s3_bucket" "receipts" {
  bucket        = var.bucket_name
  force_destroy = true 
}

resource "aws_dynamodb_table" "receipts_table" {
  name         = var.table_name
  billing_mode = "PAY_PER_REQUEST" 
  hash_key     = "Id"     

  attribute {
    name = "Id"
    type = "S"
  }

}