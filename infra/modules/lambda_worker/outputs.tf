output "queue_url" {
  value       = aws_sqs_queue.queue.id
  description = "URL stworzonej kolejki SQS"
}

output "queue_arn" {
  value       = aws_sqs_queue.queue.arn
  description = "ARN stworzonej kolejki SQS"
}

output "role_name"{
  value = aws_iam_role.lambda_role.name
  description = "Lambda default role"
}