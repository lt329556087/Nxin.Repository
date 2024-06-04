FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf
ENV ASPNETCORE_HOSTINGSTARTUPASSEMBLIES SkyAPM.Agent.AspNetCore
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
# WORKDIR Shared
WORKDIR /FinanceManagement/src
COPY ["dbn.fmservice/NuGet.Config","/FinanceManagement/src"]
COPY ["./dbn.fmservice/","/FinanceManagement/src"]
RUN dotnet restore ./FinanceManagement.ApiHost/FinanceManagement.ApiHost.csproj
COPY . .
RUN ls
WORKDIR ./FinanceManagement.ApiHost/
RUN dotnet build ./FinanceManagement.ApiHost.csproj -c Release -o /app


FROM build AS publish
RUN dotnet publish ./FinanceManagement.ApiHost.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FinanceManagement.ApiHost.dll"]
