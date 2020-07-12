pushd UsageLogger.Web
dotnet publish -c Release
popd

docker build -t perlun/usagelogger-web-api .
