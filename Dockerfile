FROM mcr.microsoft.com/dotnet/aspnet:9.0

ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /opt/TOMs/IoTCenter
COPY /Release .

ENV ASPNETCORE_URLS=https://+:44380;http://+:44381
EXPOSE 44380 44381

COPY /TOMs.sh /TOMs.sh
RUN chmod +x /TOMs.sh

ENTRYPOINT ["/TOMs.sh"]