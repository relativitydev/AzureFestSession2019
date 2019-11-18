using System;

namespace Face_Recognition
{
  public class Constants
  {
    public class AzureFaceRecognition
    {
      public static readonly string FaceSubscriptionKey = "4817634187714bd59c3b880f65436aa6";
      public static readonly string FaceEndpoint = "https://toshearer-face.cognitiveservices.azure.com";
      public const int THREAD_SLEEP = 3000;
    }

    public class GuidsAbc
    {
      public class Application
      { 
        public static readonly Guid ApplicationGuid = new Guid("8E79AAB5-016A-4758-ADC5-EE81FA5B7500");
      }

      public class Tabs
      {
        public static readonly Guid FaceRecognitionGroup = new Guid("A6124B5D-E7AD-444C-AAE7-6D3E917BDF77");
        public static readonly Guid FaceRecognitionPerson = new Guid("CA7541E0-F296-4FF5-B652-17E0C26EB878");
        public static readonly Guid FaceRecognitionImage = new Guid("28A5BDC4-D907-4C51-9C68-913726B80BD7");
        public static readonly Guid FaceRecognitionResults = new Guid("	79B4943D-4397-48A7-8CAB-0AE0CA93723B");
      }

      public class ObjectType
      {
        public static readonly Guid Document = new Guid("15C36703-74EA-4FF8-9DFB-AD30ECE7530D");
        public static readonly Guid FaceRecognitionGroup = new Guid("60543305-CE56-403E-A4D3-ABC3279134D6");
        public static readonly Guid FaceRecognitionPerson = new Guid("8F415173-DF14-4B7C-9E24-788A871FC767");
        public static readonly Guid FaceRecognitionImage = new Guid("F8075860-C68C-4571-9569-725CF36E99A9");
        public static readonly Guid FaceRecognitionResults = new Guid("26625CBD-813D-4603-8A21-01C0D8497028");
      }

      public class Field
      {
        public class FaceRecognitionGroup
        {
          public static readonly Guid ArtifactId = new Guid("");
          public static readonly Guid Name = new Guid("91A3E8B1-95EB-4560-B40D-A5935B01A068");
          public const string Name1 = "91A3E8B1-95EB-4560-B40D-A5935B01A068";
          public static string Name2 = "91A3E8B1-95EB-4560-B40D-A5935B01A068";
          public static readonly Guid GroupId = new Guid("AC4AE5DF-3754-442C-926A-DCEFE4117593");
          public static readonly Guid TrainingStatus = new Guid("8E537153-F0AD-4231-A14E-0E37667909A9");
          public static readonly Guid SystemCreatedOn = new Guid("");
          public static readonly Guid SystemLastModifiedOn = new Guid("");
          public static readonly Guid SystemCreatedBy = new Guid("");
          public static readonly Guid SystemLastModifiedBy = new Guid("");
        }

        public class FaceRecognitionPerson
        {
          public static readonly Guid ArtifactId = new Guid("");
          public static readonly Guid Name = new Guid("F4A8E81A-7A8D-4AF4-AE83-14FB4454B8BE");
          public static readonly Guid Group = new Guid("2194E165-DF56-42D5-B2AD-0400709E9C29");
          public static readonly Guid PersonId = new Guid("40BB1D1E-733F-4B54-9707-F8696E3A1E7B");
          public static readonly Guid Images = new Guid("C597F8DE-DE1A-425A-A9B7-65195A8712F7");
          public static readonly Guid SystemCreatedOn = new Guid("");
          public static readonly Guid SystemLastModifiedOn = new Guid("");
          public static readonly Guid SystemCreatedBy = new Guid("");
          public static readonly Guid SystemLastModifiedBy = new Guid("");
        }

        public class FaceRecognitionImage
        {
          public static readonly Guid ArtifactId = new Guid("");
          public static readonly Guid Name = new Guid("A5D7BCCA-719B-4D64-AF70-BEAC14367A48");
          public static readonly Guid Group = new Guid("B965E729-D88D-4346-B38A-8E9F9A934BA5");
          public static readonly Guid Images = new Guid("ED42017D-9582-4C0D-BBD9-E8DBC36E4BD1");
          public static readonly Guid Results = new Guid("ED49C268-0464-4840-9DE9-5F645CD7211F");
          public static readonly Guid SystemCreatedOn = new Guid("");
          public static readonly Guid SystemLastModifiedOn = new Guid("");
          public static readonly Guid SystemCreatedBy = new Guid("");
          public static readonly Guid SystemLastModifiedBy = new Guid("");
        }

        public class FaceRecognitionResults
        {
          public static readonly Guid ArtifactId = new Guid("");
          public static readonly Guid Name = new Guid("9597CEF5-1E4B-42CB-A753-6624E65FACF7");
          public static readonly Guid Confidence = new Guid("743E379E-5F8A-4562-84A0-C649D27C9289");
          public static readonly Guid PersonId = new Guid("F35D22B3-0B8B-4711-B607-7CD346FD3A61");
          public static readonly Guid PersonName = new Guid("2AFEDD86-8FD2-423E-B914-F1746A9B9018");
          public static readonly Guid Results = new Guid("633C8FB4-9533-4031-8D96-77B99B834A25");
          public static readonly Guid SystemCreatedOn = new Guid("");
          public static readonly Guid SystemLastModifiedOn = new Guid("");
          public static readonly Guid SystemCreatedBy = new Guid("");
          public static readonly Guid SystemLastModifiedBy = new Guid("");
        }

      }
    }
  }
}
