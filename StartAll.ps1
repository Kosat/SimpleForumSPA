pushd .
cd .\WebSPA\ClientApp
npm update
npm install
# WORKAROUND for angular2-logge plugin build problem
Remove-Item -Path .\node_modules\angular2-logger\app\core\*.ts -Force
popd

Start-Process -FilePath "dotnet" -ArgumentList "run --project .\Api.Auth\Api.Auth.csproj"
Start-Process -FilePath "dotnet" -ArgumentList "run --project .\Api.Forum\Api.Forum.csproj"
Start-Process -FilePath "dotnet" -ArgumentList "run --project .\WebSPA\WebSPA.csproj"
