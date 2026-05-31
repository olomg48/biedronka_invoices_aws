module "storage" {
  source = "./modules/storage"
  bucket_name = var.bucket_name
  table_name  = var.table_name
}

module "ocr_worker"{
  source = "./modules/lambda_worker"
  lambda_zip_path = "mock_lambda.zip"
  worker_name = "ocr_worker"
}

module "llm_worker"{
  source = "./modules/lambda_worker"
  lambda_zip_path = "mock_lambda.zip"
  worker_name = "llm_worker"
}

module "db_worker"{
  source = "./modules/lambda_worker"
  lambda_zip_path = "mock_lambda.zip"
  worker_name = "db_worker"
}
resource "aws_sqs_queue_policy" "allow_s3_to_sqs1" {
  queue_url = module.ocr_worker.queue_url 
  policy = jsonencode({
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "S3Publish",
      "Effect": "Allow",
      "Principal": {
        "Service": "s3.amazonaws.com" 
      },
      "Action": [
        "sqs:SendMessage"
      ],
      "Resource": module.storage.bucket_arn
      
    }
  ]
})
}

resource "aws_s3_bucket_notification" "bucket_notification" {
  bucket = module.storage.bucket_name
  queue {
    queue_arn     = module.ocr_worker.queue_arn
    events        = ["s3:ObjectCreated:*"]
    filter_prefix = "raw/" 
    filter_suffix = ".pdf" 
  }
}
resource "aws_iam_policy" "lambda_s3_access" {
  name        = "lambda-s3-access-policy"
  description = "RW permissions for lambdas on S3"

  policy = jsonencode({
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Action": [
          "s3:GetObject",
          "s3:PutObject"
        ],
        "Resource": "${module.storage.bucket_arn}/*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "ocr_s3" {
  role       = module.ocr_worker.role_name
  policy_arn = aws_iam_policy.lambda_s3_access.arn
}

resource "aws_iam_role_policy_attachment" "llm_s3" {
  role       = module.llm_worker.role_name
  policy_arn = aws_iam_policy.lambda_s3_access.arn
}

resource "aws_iam_role_policy_attachment" "db_s3" {
  role       = module.db_worker.role_name
  policy_arn = aws_iam_policy.lambda_s3_access.arn
}