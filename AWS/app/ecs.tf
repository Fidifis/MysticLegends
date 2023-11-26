# TODO: move to module

locals {
  ecs_cluster_name = "${local.common_prefix}-cluster"
  container_name   = "mysticlegends-server-container"
}

data "aws_iam_policy_document" "ecs_node" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ec2.amazonaws.com"]
    }
  }
}

data "aws_iam_policy_document" "logging" {
  statement {
    actions = [
      "logs:CreateLogStream",
      "logs:DescribeLogStreams",
      "logs:PutLogEvents",
      "logs:GetLogEvents"
    ]
    effect    = "Allow"
    resources = ["*"]
  }
}

resource "aws_iam_role" "ecs_node" {
  name               = "${local.common_prefix}-ecs-agent"
  assume_role_policy = data.aws_iam_policy_document.ecs_node.json

  inline_policy {
    name   = "cloudwatchlogs"
    policy = data.aws_iam_policy_document.logging.json
  }

  tags = { Module = "ecs" }
}

resource "aws_iam_role_policy_attachment" "ecs_node_container_service" {
  role       = aws_iam_role.ecs_node.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"
}

resource "aws_iam_role_policy_attachment" "ecs_node_ssm_instance" {
  role       = aws_iam_role.ecs_node.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

resource "aws_iam_instance_profile" "ecs_node" {
  name = "${local.common_prefix}-ecs-agent"
  role = aws_iam_role.ecs_node.name
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

resource "aws_launch_template" "ecs" {
  instance_type          = "t3a.nano"
  image_id               = data.aws_ami.ec2_amz23_ami.image_id
  user_data              = base64encode("#!/bin/bash\necho ECS_CLUSTER='${local.ecs_cluster_name}' >> /etc/ecs/ecs.config")
  update_default_version = true

  iam_instance_profile {
    arn = aws_iam_instance_profile.ecs_node.arn
  }

  network_interfaces {
    security_groups = [module.ecs_node_segr.security_groups_ids.ecs_node]
  }
}

resource "aws_ecs_cluster" "ecs_cluster" {
  name = local.ecs_cluster_name
  tags = { Module = "ecs" }
}

resource "aws_ecs_task_definition" "task_definition" {
  family                   = "mysticlegends-server"
  requires_compatibilities = ["EC2"]
  network_mode             = "awsvpc"
  container_definitions = jsonencode(
    [
      {
        "name" : "${local.container_name}",
        "cpu" : 1024,
        "memory" : 512,
        "memoryReservation" : 256,
        "essential" : true,
        "logConfiguration": {
            "logDriver": "awslogs",
            "options": {
               "awslogs-group" : "${aws_cloudwatch_log_group.container_logs.name}",
               "awslogs-region": "${var.meta.region}",
               "awslogs-stream-prefix": "ecs"
            }
         },
        "portMappings" : [
          {
            "containerPort" : 80,
            "hostPort" : 80,
            "protocol" : "tcp"
          },
          {
            "containerPort" : 443,
            "hostPort" : 443,
            "protocol" : "tcp"
          }
        ],
        "image" : "956941652442.dkr.ecr.eu-west-1.amazonaws.com/mysticlegends-server:v0.0.1",
        "environment" : [
          {
            "name" : "CONNECTIONSTRING",
            "value" : "Server=db.kii.pef.czu.cz;Port=5432;Database=xdigf001;User Id=xdigf001;Password=nbs0e2;Pooling=true;MaxPoolSize=6"
          }
        ]
      }
    ]
  )
  tags = { Module = "ecs" }
}

resource "aws_autoscaling_group" "ecs_asg" {
  name                = "asg"
  vpc_zone_identifier = module.vpc.subnet_ids.public

  min_size                  = 0
  max_size                  = 1
  desired_capacity          = 0
  health_check_grace_period = 300
  health_check_type         = "EC2"

  launch_template {
    id = aws_launch_template.ecs.id
    #version = "$Latest"
  }

  lifecycle {
    ignore_changes = [
      desired_capacity,
    ]
  }

  tag {
    key                 = "AmazonECSManaged"
    value               = true
    propagate_at_launch = true
  }

  tag {
    key                 = "Module"
    value               = "ecs"
    propagate_at_launch = true
  }
}

resource "aws_ecs_capacity_provider" "ecs_capacity_provider" {
  name = "ec2-capacitator"

  auto_scaling_group_provider {
    auto_scaling_group_arn = aws_autoscaling_group.ecs_asg.arn

    managed_scaling {
      instance_warmup_period    = 120
      maximum_scaling_step_size = 2
      minimum_scaling_step_size = 1
      status                    = "ENABLED"
      target_capacity           = 100
    }
  }
  tags = { Module = "ecs" }
}

resource "aws_ecs_cluster_capacity_providers" "ecs_cluster_capacity" {
  cluster_name = aws_ecs_cluster.ecs_cluster.name

  capacity_providers = [aws_ecs_capacity_provider.ecs_capacity_provider.name]

  default_capacity_provider_strategy {
    base              = 1
    weight            = 1
    capacity_provider = aws_ecs_capacity_provider.ecs_capacity_provider.name
  }
}

resource "aws_ecs_service" "ecs_service" {
  name            = "mysticlegends-server"
  cluster         = aws_ecs_cluster.ecs_cluster.id
  task_definition = aws_ecs_task_definition.task_definition.arn

  desired_count                     = 1
  health_check_grace_period_seconds = 300

  network_configuration {
    subnets         = module.vpc.subnet_ids.public
    security_groups = [module.ecs_node_segr.security_groups_ids.ecs_node]
  }

  force_new_deployment = true
  placement_constraints {
    type = "distinctInstance"
  }

  # triggers = {
  #   redeployment = timestamp()
  # }

  load_balancer {
    target_group_arn = aws_lb_target_group.server_target.arn
    container_name   = local.container_name
    container_port   = 80
  }

  capacity_provider_strategy {
    capacity_provider = aws_ecs_capacity_provider.ecs_capacity_provider.name
    base              = 1
    weight            = 1
  }

  deployment_circuit_breaker {
    enable   = false
    rollback = false
  }

  deployment_controller {
    type = "ECS"
  }

  tags = { Module = "ecs" }
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
    target_group_arn = aws_lb_target_group.server_target.arn
  }
}

resource "aws_lb_target_group" "server_target" {
  name        = "ecs-target-group"
  port        = 80
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = module.vpc.vpc_id

  health_check {
    path = "/api/health"
  }
}

resource "aws_cloudwatch_log_group" "container_logs" {
  name = "/mysticlegends/server"

  tags = {
    Module = "ecs"
  }
}
