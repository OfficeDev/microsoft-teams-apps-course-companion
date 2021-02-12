# Copyright (c) Microsoft Corporation.
# Licensed under the MIT license.

# The TenantId in which to create these objects
$tenantId = Read-Host -Prompt "Enter the Tenant ID"

# The SubscriptionId in which to create these objects
$SubscriptionId = Read-Host -Prompt "Enter the Subscription ID"

$baseResourceName = Read-Host -Prompt "Enter a base resource name that is used to generate Azure resource names"
$location = Read-Host -Prompt "Enter the location (i.e. centralus)"
$resourceGroupName = Read-Host -Prompt "Enter a Resource Group Name"
$storageAccountName = "${baseResourceName}storage"
$containerName = "${baseResourceName}container"
$bacpacFileName = "coursecompanion.bacpac"
$bacpacUrl = Read-Host "Enter raw file path from GitHub (e.g. https://github.com/OfficeDev/microsoft-teams-apps-course-companion/blob/master/Deployment/coursecompanion.bacpac?raw=true"

Connect-AzAccount -TenantId $tenantId

Set-AzContext -SubscriptionId $SubscriptionId

# Download the bacpac file
Invoke-WebRequest -Uri $bacpacUrl -OutFile "$HOME/$bacpacFileName"

# Create a storage account
$storageAccount = New-AzStorageAccount -ResourceGroupName $resourceGroupName `
-Name $storageAccountName `
-SkuName Standard_LRS `
-Location $location
$storageAccountKey = (Get-AzStorageAccountKey -ResourceGroupName $resourceGroupName `
-Name $storageAccountName).Value[0]

# Create a container
New-AzStorageContainer -Name $containerName -Context $storageAccount.Context

# Upload the BACPAC file to the container
Set-AzStorageBlobContent -File $HOME/$bacpacFileName `
-Container $containerName `
-Blob $bacpacFileName `
-Context $storageAccount.Context

Write-Host "The storage account name is $storageAccountName"
Write-Host "The container name is $containerName"
Write-Host "The storage account key is $storageAccountKey"
Write-Host "The BACPAC file URL is https://$storageAccountName.blob.core.windows.net/$containerName/$bacpacFileName"
Write-Host "Press [ENTER] to continue ..."