FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY /release .

ENV ASPNETCORE_URLS=https://+:44380;http://+:44381
EXPOSE 44380 44381


CMD ["sh", "/app/shells/restart.sh"]
ENTRYPOINT ["dotnet", "/app/IoTCenterWeb/publish/IoTCenterWebApi.dll"]