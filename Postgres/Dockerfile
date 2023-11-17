FROM postgres:16.1

ENV POSTGRES_USER xdigf001
ENV POSTGRES_PASSWORD docker
ENV POSTGRES_DB xdigf001

COPY ["db init script.sql", "/docker-entrypoint-initdb.d/"]
