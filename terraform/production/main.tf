provider "aws" {
  region  = "eu-west-2"
  version = "~> 2.0"
}
data "aws_caller_identity" "current" {}
data "aws_region" "current" {}
locals {
   application_name = "repairs api"
   parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-housing-production"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/repairs-api/state"
  }
}

/*    POSTGRES SET UP    */
data "aws_vpc" "production_vpc" {
  tags = {
    Name = "vpc-housing-production"
  }
}
data "aws_subnet_ids" "production" {
  vpc_id = data.aws_vpc.production_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}

 data "aws_ssm_parameter" "repairs_postgres_db_password" {
   name = "/repairs-api/production/postgres-password"
 }

 data "aws_ssm_parameter" "repairs_postgres_username" {
   name = "/repairs-api/production/postgres-username"
 }

module "postgres_db_production" {
  source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
  environment_name = "production"
  vpc_id = data.aws_vpc.production_vpc.id
  db_identifier = "repairs-db"
  db_name = "repairs_db"
  db_port  = 5830
  subnet_ids = data.aws_subnet_ids.production.ids
  db_engine = "postgres"
  db_engine_version = "12.5"
  db_instance_class = "db.t3.medium"
  db_allocated_storage = 100
  maintenance_window = "sun:01:00-sun:01:30"
  db_username = data.aws_ssm_parameter.repairs_postgres_username.value
  db_password = data.aws_ssm_parameter.repairs_postgres_db_password.value
  storage_encrypted = true
  multi_az = true //only true if production deployment
  publicly_accessible = false
  project_name = "repairs hub"
}
