variable "bucket_name" {
  type        = string
  description = "Nazwa bucketu przekazywana z tfvars"
}

variable "table_name" {
  type        = string
  description = "Nazwa tabeli przekazywana z tfvars"
}

variable "mistral_key" {
  type        = string
  description = "Nazwa tabeli przekazywana z tfvars"
  sensitive = true
}