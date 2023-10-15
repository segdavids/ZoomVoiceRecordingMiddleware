FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build


RUN mkdir -p /app

WORKDIR /app 



EXPOSE 443
EXPOSE 80
EXPOSE 5000

COPY bin/Release/net6.0/publish .

ENTRYPOINT ["dotnet", "Eleveo-EFCX-Connector-API.dll"]










# Copy everything into the docker directory and build
#COPY . .

# copy project csproj file and restore it in docker directory
#COPY ./*.csproj ./


#RUN dotnet restore


#RUN dotnet publish -c Release -o out

# Build runtime final image
#FROM mcr.microsoft.com/dotnet/aspnet:6.0
#WORKDIR /app
#COPY --from=build /app/out .
#ENTRYPOINT ["dotnet", "Eleveo-EFCX-Connector-API.dll"]
