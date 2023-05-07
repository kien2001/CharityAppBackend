# CharityAppBackend

Để chạy project cần cài .Net 7 và docker
- Link cài .Net 7 : https://dotnet.microsoft.com/en-us/download

Lần đầu pull code về, chạy lần lượt câu lệnh:
- dotnet build CharityAppBackend.sln
- docker-compose build
- docker compose up

Kể từ lần sau, chạy lần lượt câu lệnh:
- docker-compose down
- dotnet build CharityAppBackend.sln
- docker-compose build
- docker compose up
