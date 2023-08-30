# Service Discovery With Consul and Ocelot ApiGateway

# prepare consul on docker

docker run -d -p 8500:8500 -p 8600:8600/udp --name=my-consul consul:1.15.4 agent -server -ui -node=server-1 -bootstrap-expect=1 -client=0.0.0.0

# prepare dotnet project

dotnet new sln --name ConsulOcelotDemo

dotnet new webapi --name OcelotApiGateway
dotnet sln ConsulOcelotDemo.sln add OcelotApiGateway/OcelotApiGateway.csproj

dotnet new webapi --name ProductApi
dotnet sln ConsulOcelotDemo.sln add ProductApi/ProductApi.csproj

dotnet build

cd OcelotApiGateway
dotnet add package Ocelot.Provider.Consul --version 19.0.2
cd ..

cd ProductApi
dotnet add package Consul --version 1.6.10.9
cd ..

dotnet build

# after development run these scripts

dotnet OcelotApiGateway.dll

dotnet ProductAPI.dll --urls=http://localhost:5401 --ApiNumber=1
dotnet ProductAPI.dll --urls=http://localhost:5402 --ApiNumber=2
dotnet ProductAPI.dll --urls=http://localhost:5403 --ApiNumber=3

# call these urls

http://localhost:5000/v1/api/product

http://localhost:5000/v1/api/product/1
