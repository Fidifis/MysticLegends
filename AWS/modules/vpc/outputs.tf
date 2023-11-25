output "vpc_id" {
  value = aws_vpc.this.id
}

output "subnet_ids" {
  value = {
    public = { for k, v in aws_subnet.public : k => v.id }
    nat = { for k, v in aws_subnet.nat : k => v.id }
    private = { for k, v in aws_subnet.private : k => v.id }
  }
}
