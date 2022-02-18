FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["InOneWeekend/InOneWeekend.csproj", "InOneWeekend/"]
RUN dotnet restore "InOneWeekend/InOneWeekend.csproj"
COPY . .
WORKDIR "/src/InOneWeekend"
RUN dotnet build "InOneWeekend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InOneWeekend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InOneWeekend.dll"]
