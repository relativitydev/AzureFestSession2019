
# From Recognition Relativity Application Setup #
* Set up Azure
    * Instructions can be found here: https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/docs/from_recognition_setup.md
* Install Application into Relativity Workspace
    *  Download application here: https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/FormRecognition/Rap/RA_Kramerica_20191010002915.rap
   *  Install into Relativity Application Library
    *  Install into Relativity Workspace
* Configure Relativity Workspace
  * Select Form Regognizer --> Azure Form Recognition Setting tab
    * Give it a name
    * Enter the Cognitive Serives Key
    * Enter the Cognitive Services Endpoint
    * Enter the Storage Account Name
    * Enter the Storage Account Key
  * ![Azure From Recognition Settings](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/form_recognizer_settings.png?raw=true)
  * **The settings value will disappear** - There is a presave event handler that stores the values in the secret store and removes them from the settings object so they are not visible to users.
  
