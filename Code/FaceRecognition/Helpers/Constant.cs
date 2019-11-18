using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.EventHandler.StatisticsEventHandler;

namespace FaceRecognition.Helpers
{
#pragma warning disable RG0001
#pragma warning disable RG2002

	public class Constant
	{
		public class Group
		{
			public const string COMPLETE = "Complete";
			public const string INCOMPLETE = "Incomplete";
			public const string GROUP_CREATION_STATUS_DESC = "Group Creation ";
			public const string GROUP_DELETION_STATUS_DESC = "Group Deletion ";
			public const string GROUP_TRAINING_STATUS_DESC = "Group Training ";
		}

		public class Person
		{
			public const string COMPLETE = "Complete";
			public const string INCOMPLETE = "Incomplete";
			public const string NO_PEOPLE_DETECTED = "No People Detected";
			public static readonly string IMAGE_DETECTION_STATUS_DESC = "Image Detection ";
		}

		public class Image
		{
			public const string RESULTS_STATUS_DESC = "Results ";
			public const string COMPLETE = "Complete";
			public const string INCOMPLETE = "Incomplete";
		}

		public class Guids
		{
			public class Application
			{
				public static readonly Guid APPLICATION_GUID = new Guid("8E79AAB5-016A-4758-ADC5-EE81FA5B7500");
			}

			public class Tabs
			{
				public static readonly Guid FACE_RECOGNITION_GROUP = new Guid("A6124B5D-E7AD-444C-AAE7-6D3E917BDF77");
				public static readonly Guid FACE_RECOGNITION_PERSON = new Guid("CA7541E0-F296-4FF5-B652-17E0C26EB878");
				public static readonly Guid FACE_RECOGNITION_IMAGE = new Guid("28A5BDC4-D907-4C51-9C68-913726B80BD7");
				public static readonly Guid FACE_RECOGNITION_RESULTS = new Guid("	79B4943D-4397-48A7-8CAB-0AE0CA93723B");
			}

			public class ObjectType
			{
				public static readonly Guid DOCUMENT = new Guid("15C36703-74EA-4FF8-9DFB-AD30ECE7530D");
				public static readonly Guid FACE_RECOGNITION_GROUP = new Guid("60543305-CE56-403E-A4D3-ABC3279134D6");
				public static readonly Guid FACE_RECOGNITION_PERSON = new Guid("8F415173-DF14-4B7C-9E24-788A871FC767");
				public static readonly Guid FACE_RECOGNITION_IMAGE = new Guid("F8075860-C68C-4571-9569-725CF36E99A9");
				public static readonly Guid FACE_RECOGNITION_RESULTS = new Guid("26625CBD-813D-4603-8A21-01C0D8497028");
			}

			public class Field
			{
				public class FaceRecognitionGroup
				{
					public static readonly Guid NAME = new Guid("91A3E8B1-95EB-4560-B40D-A5935B01A068");
					public static readonly Guid GROUP_ID = new Guid("AC4AE5DF-3754-442C-926A-DCEFE4117593");
					public static readonly Guid TRAINING_STATUS = new Guid("8E537153-F0AD-4231-A14E-0E37667909A9");
				}

				public class FaceRecognitionPerson
				{
					public static readonly Guid NAME = new Guid("F4A8E81A-7A8D-4AF4-AE83-14FB4454B8BE");
					public static readonly Guid GROUP = new Guid("2194E165-DF56-42D5-B2AD-0400709E9C29");
					public static readonly Guid PERSON_ID = new Guid("40BB1D1E-733F-4B54-9707-F8696E3A1E7B");
					public static readonly Guid IMAGES = new Guid("C597F8DE-DE1A-425A-A9B7-65195A8712F7");
					public static readonly Guid STATUS = new Guid("92D8D6FA-A901-47CE-9D6D-CCED29907AF0");
				}

				public class FaceRecognitionImage
				{
					public static readonly Guid NAME = new Guid("A5D7BCCA-719B-4D64-AF70-BEAC14367A48");
					public static readonly Guid GROUP = new Guid("B965E729-D88D-4346-B38A-8E9F9A934BA5");
					public static readonly Guid IMAGE = new Guid("ED42017D-9582-4C0D-BBD9-E8DBC36E4BD1");
					public static readonly Guid RESULTS = new Guid("ED49C268-0464-4840-9DE9-5F645CD7211F");
					public static readonly Guid STATUS = new Guid("A841D49F-A97A-42E9-BDE4-1D3BDF2E067C");
				}

				public class FaceRecognitionResults
				{
					public static readonly Guid NAME = new Guid("9597CEF5-1E4B-42CB-A753-6624E65FACF7");
					public static readonly Guid PERSON_NAME = new Guid("4C22770A-59BA-4C6F-9535-36A239B82DB8");
					public static readonly Guid CONFIDENCE = new Guid("743E379E-5F8A-4562-84A0-C649D27C9289");
					public static readonly Guid RESULTS = new Guid("633C8FB4-9533-4031-8D96-77B99B834A25");
				}
			}
		}
	}
}
