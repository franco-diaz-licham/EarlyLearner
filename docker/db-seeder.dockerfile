FROM mcr.microsoft.com/powershell:7.5-debian-12

WORKDIR /scripts

RUN apt-get update \
    && apt-get install -y --no-install-recommends postgresql-client \
    && rm -rf /var/lib/apt/lists/*

COPY ./scripts/init-db.ps1 ./init-db.ps1
COPY ./scripts/seed.sql ./seed.sql

ENTRYPOINT ["pwsh", "-NoLogo", "-NoProfile", "-File", "./init-db.ps1"]
