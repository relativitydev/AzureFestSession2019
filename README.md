# AzureFestSession2019
This is the repository that will hold all of the source code and documentation for the "Enhance your Relativity Applications Using Azure Cloud Services


This solution contains the source code for three different applications:
- Face Recognition
- Form Recognizer (Includes Bing Search and Bing Maps)
- Video Indexer

There is also a RAP included, which will have all of the tabs, objects, views, etc needed to recreate this application in your Relativity environment. These are ONLY compatible with Relativity version 9.2 and higher. In order to attach the event handlers and agents needed to properly execute the application, there are certain changes that you will need to make in the solution. 
These can be easily located in Visual Studio by going into "View" and selecting "Task List". All of the changes that you will need to make are commented by "TODO"
![Task List](https://github.com/relativitydev/AzureFestSession2019/blob/master/Images/Task_List.png)

## Face Recognition
Check the likelihood that two faces belong to the same person. The API will return a confidence score about how likely it is that the two faces belong to one person.

## Form Recognition
Uses the limited-access preview of the Azure Form Recognizer. Azure Form Recognizer is a cognitive service that uses machine learning technology to identify and extract key-value pairs and table data from form documents. It then outputs structured data that includes the relationships in the original file. Unsupervised learning allows the model to understand the layout and relationships between fields and entries without manual data labeling or intensive coding and maintenance.

## Video Indexer 
Automatically extract metadata—such as spoken words, written text, faces, speakers, celebrities, emotions, topics, brands, and scenes—from video and audio files. Access the data within your application or infrastructure, make it more discoverable, and use it to create new over-the-top (OTT) experiences and monetization opportunities.     



## Acknowledgments 
Thank you to Todd Shearer, Mark Robustelli, Erik Naujokas, Michael Obregon, Jim Brennan, Luke Miller, Daniel Eimer, Nick Kapuza and Chris Hogan and the countless other Relativians who made this session possible :) 
