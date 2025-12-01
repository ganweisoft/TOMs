## dapr的一些启动参数要求及说明

### 目录层级

```
│  appsettings.json
│  AutoMapper.dll
│  Dapr.AspNetCore.dll
│  Dapr.Client.dll
│  Dapr.Common.dll
│  Dapr.Protos.dll
│  dapr.yaml
│  EntityFramework.dll
│  EntityFramework.SqlServer.dll
│  Google.Api.CommonProtos.dll
│  Google.Protobuf.dll
│  Grpc.AspNetCore.HealthChecks.dll
│  Grpc.AspNetCore.Server.ClientFactory.dll
│  Grpc.AspNetCore.Server.dll
│  Grpc.AspNetCore.Web.dll
│  Grpc.Core.Api.dll
│  Grpc.HealthCheck.dll
│  Grpc.Net.Client.dll
│  Grpc.Net.ClientFactory.dll
│  Grpc.Net.Common.dll
│  GWHost1.deps.json
│  GWHost1.dll
│  GWHost1.exe
|  ......
```

### 运行命令

```shell
dapr run -f .
```

### 加入鉴权

文档链接: https://docs.dapr.io/zh-hans/operations/security/app-api-token/