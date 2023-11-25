# TODO: move to module

locals {
  ecs_cluster_name = "${local.common_prefix}-cluster"
}

data "aws_iam_policy_document" "ecs_agent" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ec2.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "ecs_agent" {
  name               = "${local.common_prefix}-ecs-agent"
  assume_role_policy = data.aws_iam_policy_document.ecs_agent.json
  tags               = { Module = "ecs" }
}

resource "aws_iam_role_policy_attachment" "ecs_agent" {
  role       = aws_iam_role.ecs_agent.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"
}

resource "aws_iam_instance_profile" "ecs_agent" {
  name = "${local.common_prefix}-ecs-agent"
  role = aws_iam_role.ecs_agent.name
}

data "aws_ami" "ec2_amz23_ami" {
  most_recent = true

  filter {
    name   = "name"
    values = ["al2023-ami-ecs-hvm-2023.*-x86_64"]
  }

  filter {
    name   = "owner-alias"
    values = ["amazon"]
  }
}

module "ec_alb_segr" {
  source = "../modules/security-groups"
  meta   = var.meta
  prefix = local.common_prefix

  vpc_id = module.vpc.vpc_id
  open_ports = {
    ecs_alb = {
      ingress     = [80, 443]
      protocol    = "tcp"
      open_egress = false
    }
  }
}

module "ecs_node_segr" {
  source = "../modules/security-groups"
  meta   = var.meta
  prefix = local.common_prefix

  vpc_id = module.vpc.vpc_id
  open_ports = {
    ecs_node = {
      ingress         = [80, 443]
      protocol        = "tcp"
      open_egress     = true
      security_groups = [module.ec_alb_segr.security_groups_ids.ecs_alb]
    }
  }
}

resource "aws_launch_configuration" "ecs_launch_config" {
  associate_public_ip_address = false
  image_id                    = data.aws_ami.ec2_amz23_ami.image_id
  iam_instance_profile        = aws_iam_instance_profile.ecs_agent.name
  security_groups             = [module.ecs_node_segr.security_groups_ids.ecs_node]
  user_data                   = "#!/bin/bash\necho ECS_CLUSTER='${local.ecs_cluster_name}' >> /etc/ecs/ecs.config"
  instance_type               = "t3a.nano"
}

resource "aws_autoscaling_group" "ecs_asg" {
  name                 = "asg"
  vpc_zone_identifier  = module.vpc.subnet_ids.public
  launch_configuration = aws_launch_configuration.ecs_launch_config.name

  desired_capacity          = 1
  min_size                  = 1
  max_size                  = 3
  health_check_grace_period = 300
  health_check_type         = "EC2"
}

resource "aws_ecs_cluster" "ecs_cluster" {
  name = local.ecs_cluster_name
  tags = { Module = "ecs" }
}

resource "aws_ecs_task_definition" "task_definition" {
  family                = "worker"
  network_mode          = "awsvpc"
  container_definitions = jsonencode(
    [
      {
        "memory": 512,
        "portMappings": [
            {
               "containerPort": 80,
               "hostPort": 80,
               "protocol": "tcp"
            }
         ]
        "memoryReservation": 256,
        "name": "worker",
        "cpu": 1024,
        "image": "956941652442.dkr.ecr.eu-west-1.amazonaws.com/mysticlegends-server",
        "environment": []
      }
    ]
  )
  tags = { Module = "ecs" }
}

resource "aws_ecs_capacity_provider" "ecs_capacity_provider" {
  name = "test1"

  auto_scaling_group_provider {
    auto_scaling_group_arn = aws_autoscaling_group.ecs_asg.arn

    managed_scaling {
      maximum_scaling_step_size = 1000
      minimum_scaling_step_size = 1
      status                    = "ENABLED"
      target_capacity           = 1
    }
  }
}

resource "aws_ecs_cluster_capacity_providers" "example" {
  cluster_name = aws_ecs_cluster.ecs_cluster.name

  capacity_providers = [aws_ecs_capacity_provider.ecs_capacity_provider.name]

  default_capacity_provider_strategy {
    base              = 1
    weight            = 100
    capacity_provider = aws_ecs_capacity_provider.ecs_capacity_provider.name
  }
}

resource "aws_ecs_service" "ecs_service" {
 name            = "my-ecs-service"
 cluster         = aws_ecs_cluster.ecs_cluster.id
 task_definition = aws_ecs_task_definition.task_definition.arn
 desired_count   = 1

 network_configuration {
   subnets         = module.vpc.subnet_ids.public
   security_groups = [module.ecs_node_segr.security_groups_ids.ecs_node]
 }

 force_new_deployment = true
 placement_constraints {
   type = "distinctInstance"
 }

 triggers = {
   redeployment = timestamp()
 }

 capacity_provider_strategy {
   capacity_provider = aws_ecs_capacity_provider.ecs_capacity_provider.name
   weight            = 100
 }

 load_balancer {
   target_group_arn = aws_lb_target_group.ecs_tg.arn
   container_name   = "worker"
   container_port   = 80
 }
}

resource "aws_lb" "ecs_alb" {
 name               = "ecs-alb"
 internal           = false
 load_balancer_type = "application"
 security_groups    = [module.ec_alb_segr.security_groups_ids.ecs_alb]
 subnets            = module.vpc.subnet_ids.public

 tags = {
   Name = "ecs-alb"
 }
}

resource "aws_lb_listener" "ecs_alb_listener" {
 load_balancer_arn = aws_lb.ecs_alb.arn
 port              = 80
 protocol          = "HTTP"

 default_action {
   type             = "forward"
   target_group_arn = aws_lb_target_group.ecs_tg.arn
 }
}

resource "aws_lb_target_group" "ecs_tg" {
 name        = "ecs-target-group"
 port        = 80
 protocol    = "HTTP"
 target_type = "ip"
 vpc_id      = module.vpc.vpc_id

 health_check {
   path = "/api/health"
 }
}
