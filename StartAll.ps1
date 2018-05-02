Start-Process -FilePath "dotnet" -ArgumentList "run --project .\Api.Auth\Api.Auth.csproj"
Start-Process -FilePath "dotnet" -ArgumentList "run --project .\Api.Forum\Api.Forum.csproj"
Start-Process -FilePath "dotnet" -ArgumentList "run --project .\WebSPA\WebSPA.csproj"