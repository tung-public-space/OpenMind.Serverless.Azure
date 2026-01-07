@description('Location for all resources')
output appInsightsName string = appInsights.name
output storageAccountName string = storageAccount.name
output functionAppHostName string = functionApp.properties.defaultHostName
output functionAppName string = functionApp.name

}
  }
    httpsOnly: true
    }
      ]
        }
          value: 'dotnet-isolated'
          name: 'FUNCTIONS_WORKER_RUNTIME'
        {
        }
          value: appInsights.properties.ConnectionString
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
        {
        }
          value: appInsights.properties.InstrumentationKey
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
        {
        }
          value: '~4'
          name: 'FUNCTIONS_EXTENSION_VERSION'
        {
        }
          value: toLower(functionAppName)
          name: 'WEBSITE_CONTENTSHARE'
        {
        }
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
        {
        }
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
          name: 'AzureWebJobsStorage'
        {
      appSettings: [
      netFrameworkVersion: 'v9.0'
    siteConfig: {
    serverFarmId: appServicePlan.id
  properties: {
  kind: 'functionapp'
  location: location
  name: functionAppName
resource functionApp 'Microsoft.Web/sites@2023-01-01' = {
// Function App

}
  }
    reserved: false // Set to true for Linux
  properties: {
  }
    tier: 'Dynamic'
    name: 'Y1'
  sku: {
  location: location
  name: appServicePlanName
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
// App Service Plan (Consumption plan for serverless)

}
  }
    RetentionInDays: 90
    Request_Source: 'rest'
    Application_Type: 'web'
  properties: {
  kind: 'web'
  location: location
  name: appInsightsName
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
// Application Insights

}
  }
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  properties: {
  kind: 'StorageV2'
  }
    name: 'Standard_LRS'
  sku: {
  location: location
  name: take(storageAccountName, 24)
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
// Storage Account for Azure Functions

var appServicePlanName = '${baseName}-${environment}-plan'
var appInsightsName = '${baseName}-${environment}-insights'
var storageAccountName = replace('${baseName}${environment}${uniqueSuffix}', '-', '')
var functionAppName = '${baseName}-${environment}-${uniqueSuffix}'
var uniqueSuffix = uniqueString(resourceGroup().id)

param baseName string = 'order-service'
@description('Base name for all resources')

param environment string = 'dev'
@description('Environment name (dev, staging, prod)')

param location string = resourceGroup().location

