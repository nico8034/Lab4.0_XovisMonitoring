FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
VOLUME ["/app/Application/Services/CameraService"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY /API API/
COPY /Application  Application/
COPY /Domain  Domain/
COPY /Infrastructure Infrastructure/

RUN dotnet restore "API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV DOCKER_ENV=True
COPY --from=publish /app/publish .
#RUN mkdir -p /API/Experiments/
#RUN mkdir -p /Application/Services/CameraService/
#WORKDIR /app/Application/Services/CameraService
#COPY cameras.txt .
#WORKDIR /app
ENTRYPOINT ["dotnet", "API.dll"]


#docker run -p 8080:80 -v C:\Users\nicol\Desktop\git\Lab4.0_XovisMonitoring\Application\Services\CameraService\cameras.txt:/app/Application/Services/CameraService/cameras.txt test2
