#FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
#WORKDIR /app
FROM microsoft/dotnet:2.1-sdk AS build
EXPOSE 5003
ENV ASPNETCORE_URLS http://+:5003

WORKDIR /src
COPY WebSPA.sln ./
COPY Api.Forum/Api.Forum.csproj Api.Forum/
RUN dotnet restore Api.Forum -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Api.Forum
ENTRYPOINT ["dotnet", "run"]
#RUN dotnet build -c Release -o /app

#FROM build AS publish
#RUN dotnet publish -c Release -o /app /p:PublishWithAspNetCoreTargetManifest="false"
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Api.Forum.dll"]
