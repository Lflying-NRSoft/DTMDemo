# docker run -d --rm -p 9007:80 --name dtmdemostock registry.cn-shanghai.aliyuncs.com/nrsoft/dtmdemostock:develop

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
copy . ./ 
EXPOSE 80 443

#RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf
#RUN sed -i 's/TLSv1.2/TLSv1/g' /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "DTMStockDemo.dll"]
