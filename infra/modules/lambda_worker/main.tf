resource "aws_sqs_queue" "queue" {
  name = "${var.worker_name}-queue"
  visibility_timeout_seconds = 300
}

resource "aws_iam_role" "lambda_role" {
  name = "${var.worker_name}-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = {
        Service = "lambda.amazonaws.com"
      }
    }]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_logs" {
  role       = aws_iam_role.lambda_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_iam_role_policy_attachment" "lambda_sqs" {
  role       = aws_iam_role.lambda_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaSQSQueueExecutionRole"
}

resource "aws_cloudwatch_log_group" "log_group" {
  name              = "/aws/lambda/${var.worker_name}"
  retention_in_days = 7
}


resource "aws_lambda_function" "lambda" {
  function_name = var.worker_name
  role          = aws_iam_role.lambda_role.arn
  handler       = var.handler_name
  runtime       = "provided.al2"
  timeout = 120
  filename         = var.lambda_zip_path
  source_code_hash = filebase64sha256(var.lambda_zip_path)

  architectures = ["arm64"] # for local raspberry pi test
  dynamic "environment" {
    for_each = length(var.environment_variables) > 0 ? [1] : []
    content {
      variables = var.environment_variables
    }
  }

  depends_on = [aws_cloudwatch_log_group.log_group]
}

resource "aws_lambda_event_source_mapping" "sqs_trigger" {
  event_source_arn = aws_sqs_queue.queue.arn
  function_name    = aws_lambda_function.lambda.arn
  batch_size       = 1 
}