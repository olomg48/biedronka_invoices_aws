cd .\code
dotnet publish  .\Lambda1.PdfProcessor\aws_localstack.Lambda1.csproj -c Release -r linux-arm64 --self-contained true 
cd ..
if(Test-Path ".\infra\lambda1.zip"){
    Remove-Item .\infra\lambda1.zip
}
Compress-Archive .\code\Lambda1.PdfProcessor\bin\Release\net8.0\linux-arm64\publish\* .\infra\lambda1.zip -Force
terraform -chdir=infra apply -auto-approve