# Manage Your Education and Skills Funding Shared Email Processor Function
The Manage Your Education and Skills Funding Shared Email Processor Function app is used by for invoking GOV.UK Notify service to allow the following:

 - Retrieval of GOV.UK Notify dependent information based on the azure service bus message received from the queue and sending email notifications.
 - Storing the Audit logs related to email notifications in the Azure Table Storage.

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is a .Net 8 Isolated Worker Azure Function project utilizing an Azure Function App for deployment.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

For running the application locally, `local.settings.json` file need to be created in the `Pds.Shared.EmailProcessor.Func` project. Below, and included in the repo, there is `local.settings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Application Settings (`local.settings.json`)
```json
{
  "IsEncrypted": false,
  "Values": {
    "AdminApiClientConfiguration:ApiBaseAddress": "",
    "AdminApiClientConfiguration:AppUri": "",
    "AdminApiClientConfiguration:Authority": "",
    "AdminApiClientConfiguration:ClientId": "",
    "AdminApiClientConfiguration:ClientSecret": "",
    "AdminApiClientConfiguration:TenantId": "",
    "APPINSIGHTS_INSTRUMENTATIONKEY": "",
    "AzureKeyVaultURI": "",
    "AzureStorageConfiguration:ConnectionString": "",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "CacheEncryptionSecretKey": "",
    "Environment": "local",
    "FUNCTIONS_EXTENSION_VERSION": "~4",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "PdsApplicationInsights:Environment": "",
    "PdsApplicationInsights:InstrumentationKey": "",
    "QueueName": "",
    "RedisConfiguration:ConnectionString": "",
    "ServiceBusConnection": ""
  }
}
```
### Setting Details

- **`AdminApiClientConfiguration:ApiBaseAddress`**  
  The base URL endpoint for Admin API.

- **`AdminApiClientConfiguration:AppUri`**  
  The unique Application ID URI used as the identifier for the protected Admin API resource within the Identity Provider.

- **`AdminApiClientConfiguration:Authority`**  
  The base URL of the Identity Provider responsible for authenticating and issuing tokens for the Admin API client.

- **`AdminApiClientConfiguration:ClientId`**  
  The unique identifier assigned to the admin client application to authenticate its identity against the security provider when calling the Admin API.

- **`AdminApiClientConfiguration:ClientSecret`**  
  The secret credential used by the admin client application to securely prove its identity to the Identity Provider.

- **`AdminApiClientConfiguration:TenantId`**  
  The unique identifier that specifies the exact organization or cloud instance within the Identity Provider where the Admin API client is registered.

- **`APPINSIGHTS_INSTRUMENTATIONKEY`**  
  The key value for Application Insights resource for logging.

- **`AzureKeyVaultURI`**  
  Azure Key Vault URI for retrieving secret value for NotifyApiKeySecretName.

- **`AzureStorageConfiguration:ConnectionString`**  
  The connection string of azure storage. This is used for retrieving email template details from Azure table storage.

- **`AzureWebJobsDashboard`**  
  The core application setting used by the Azure Functions and Azure WebJobs runtime to establish a connection to an Azure Jobs dashboard.

- **`AzureWebJobsStorage`**  
  The core application setting used by the Azure Functions and Azure WebJobs runtime to establish a connection to an Azure Storage account.

- **`CacheEncryptionSecretKey`**  
  The encryption key used for while caching sensitive data.

- **`Environment`**  
  The environment which the app is running on.

- **`FUNCTIONS_EXTENSION_VERSION`**  
  The functions extension version number.

- **`FUNCTIONS_WORKER_RUNTIME`**  
  The functions runtime.

- **`PdsApplicationInsights:Environment`**  
  The environment which the app is running on for Application Insights for logging purposes.

- **`PdsApplicationInsights:InstrumentationKey`**  
  The key for Application Insights resource for logging purposes.

- **`QueueName`**  
  Azure service bus queue name to listen for new messages to process.

- **`RedisConfiguration:ConnectionString`**  
  The connection string of the azure cache for redis. (redis:6379 points to local redis container spun up by docker-compose)

- **`ServiceBusConnection`**  
  The connection string of the azure service bus.

## docker-compose

This project depends on a redis distributed cache resource for storing api requests/responses. We are unable to connect to deployed cloud resources and so a local redis container must be created via Docker in order to test full functionality local.

The docker-compose.yml file includeds the orchestration for starting both the api and redis containers.

You must select docker-compose as the startup project to ensure that all dependent resources are running in Docker to run this solution locally.

## Build and Test

To build and test locally, you can either use Visual Studio, Visual Studio Code or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

- If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
- If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.