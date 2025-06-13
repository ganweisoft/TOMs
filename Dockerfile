# Base image
FROM mcr.microsoft.com/dotnet/runtime:9.0

# Timezone configuration
ENV TZ=Asia/Shanghai
RUN apt-get update && \
    apt-get install -y tzdata && \
    ln -fs /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata

# Copy files
COPY IoTCenter /opt/ganwei/IoTCenter
COPY runGW.sh /opt/ganwei/

# Persist data volumes
VOLUME ["/var/gwiot/database", "/var/gwiot/data"]
EXPOSE 44380

# Startup script
CMD ["sh", "/opt/ganwei/runGW.sh"]