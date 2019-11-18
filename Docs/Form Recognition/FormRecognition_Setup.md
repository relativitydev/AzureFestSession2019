
# Setup Azure

### Setup Storage Account
This storage will be used to upload the forms used for training.  There is no delete function in the sample applicaiton, so once you have trained your model, you will need to manual remove the files from the storage account if you would like them removed.

* Log into you Azure account
  * https://portal.azure.com/#home
* Setup new or use existing storage account
  * Create New
    * Create new resource group
      * ![Create Resource Group](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/resource_group_create.png?raw=true)
    * Create new storage account
      * ![Create Storage Account](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/storage_account_create.png?raw=true)
* Get Storage Key
  *  ![Get Storage Key](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/access_key_retreive.png?raw=true)

### Setup Form Recognition Cognitive Service
More detail can be found here https://docs.microsoft.com/en-us/azure/cognitive-services/form-recognizer/quickstarts/dotnet-sdk  You will need to request access as it is currently in limited-access preview.

* You will eventually need your Cognitive Services Endpoint
  * It will look something like the following
    * https://mysubscriptionname-westus2-formrecognizer.cognitiveservices.azure.com/
