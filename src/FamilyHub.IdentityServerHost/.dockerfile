FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "FamilyHub.IdentityServerHost.dll"]

# Export image to tar 
WORKDIR /app/out
CMD $ docker save --output $(pipeline.workspace)/userserivce.image.tar $(imagename):$(build.buildid)