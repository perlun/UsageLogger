#
# Builds a Docker image hosting the UsageLogger.Web ASP.NET Core Web API
#

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

COPY UsageLogger.Web/bin/Release/netcoreapp3.1/publish/ /App/
WORKDIR /App
ENTRYPOINT ["dotnet", "UsageLogger.Web.dll"]
