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
    bucket  = "terraform-state-development-apis"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/repairs-api/state"
  }
}

/*    POSTGRES SET UP    */
data "aws_vpc" "development_vpc" {
  tags = {
    Name = "vpc-development-apis-development"
  }
}
data "aws_subnet_ids" "development_private_subnets" {
  vpc_id = data.aws_vpc.development_vpc.id
  filter {
    name   = "tag:environment"
    values = ["development"]
  }
}

 data "aws_ssm_parameter" "repairs_postgres_db_password" {
   name = "/repairs-api/development/postgres-password"
 }

 data "aws_ssm_parameter" "repairs_postgres_username" {
   name = "/repairs-api/development/postgres-username"
 }

module "postgres_db_development" {
  source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
  environment_name = "development"
  vpc_id = data.aws_vpc.development_vpc.id
  db_identifier = "repairs-db"
  db_name = "repairs_db"
  db_port  = 5829
  subnet_ids = data.aws_subnet_ids.development_private_subnets.id
  db_engine = "postgres"
  db_engine_version = "11.1" //DMS does not work well with v12
  db_instance_class = "db.t2.micro"
  db_allocated_storage = 20
  maintenance_window = "sun:10:00-sun:10:30"
  db_username = data.aws_ssm_parameter.repairs_postgres_username.value
  db_password = data.aws_ssm_parameter.repairs_postgres_db_password.value
  storage_encrypted = false
  multi_az = false //only true if production deployment
  publicly_accessible = false
  project_name = "repairs hub"
}
