FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY out ./

ENV TZ="Europe/Zagreb" \
    ASPNETCORE_URLS=http://*:50505 \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true \
    Auth__GitHubId="INVALID" \
    Auth__GitHubSecret="INVALID" \
    Auth__GitHubUsers__0="INVALID1" \
    Auth__GitHubUsers__1="INVALID2" \
    URL="http://localhost:5000" \
    SALT="CHANGE ME" 

EXPOSE 50505

ENTRYPOINT ["dotnet", "ciklonalozi.dll"]