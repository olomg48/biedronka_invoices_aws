variable "worker_name" {
  type        = string
  description = "Lambda name"
}

variable "lambda_zip_path" {
  type        = string
  description = "Path to lambda zip"
}

variable "handler_name" {
  type        = string
  description = "Name of dotnet handler for custom provider"
}

variable "environment_variables" {
  type        = map(string)
  description = "Env vars for lambda"
  default     = {}
}
