* Sample Invoices look like this
  * https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/FormRecognition/Code/SampleData/Forms/Invoice001.pdf
 
 
 ### In Relativity
* Create Model
  * Create Saved Search with training Invoices
  * Create New Azure Form Recognition Model 
    * Give it a name
    * Select Saved Search
    * Save
    * ![New Form Recognition Model](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/form_recognition_create_new_model.png?raw=true)
  * Train the Model
    * Select the Recognition Model that was created
    * Select the "Submit to Train"
    * The Guid and SAS URL will be populated
    * ![Train the Model](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/form_recognition_submit_to_train.png?raw=true)
  * Create New Azure From Recognition Form
    * Give it a name
    * Select a Document
    * Select the Training Model
    * Save
    * ![New Form](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/form_recognition_create_new_form.png?raw=true)
  * Extract The data from the Form
    * Select the Recognition Form that was created
    * Select "Extract Form Data"
      *  ![Extract Form Data](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/form_recognition_form_extract_data.png?raw=true)
    * The From Info section and Line Items will be added to the form
      *  ![Extract Form Data](https://github.com/relativitydev/azure-congnitive-service-fest2019/blob/master/images/form_recognition_extracted_items.png?raw=true)
    
  
