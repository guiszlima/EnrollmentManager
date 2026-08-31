FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

ENV DOTNET_USE_POLLING_FILE_WATCHER=1

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "watch", "run", "--no-hot-reload", "--project", "EnrollmentManager.API/EnrollmentManager.API.csproj", "--urls", "http://*:8080"]