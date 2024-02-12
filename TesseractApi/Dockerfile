#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
RUN apt update && apt install tesseract-ocr libtesseract-dev -y

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TesseractApi/TesseractApi.csproj", "TesseractApi/"]
RUN dotnet restore "TesseractApi/TesseractApi.csproj"
COPY . .
WORKDIR "/src/TesseractApi"
RUN dotnet build "TesseractApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TesseractApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TesseractApi.dll"]