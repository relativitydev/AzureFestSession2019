using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;

namespace FormRecognition
{
	public class AzureStorageService
	{
		private string _storageAccountName = string.Empty;
		private string _storageAccountKey = string.Empty;

		public string ContainerName { get; set; }
		public string SasUrl { get; set; }
		public AzureStorageService(string storageAccountKey, string storageAccountName)
		{
			_storageAccountKey = storageAccountKey;
			_storageAccountName = storageAccountName;
		}

		public CloudBlobClient GetClient()
		{

			StorageCredentials storageCredentials = new StorageCredentials(_storageAccountName, _storageAccountKey);
			CloudStorageAccount account = new CloudStorageAccount(storageCredentials, useHttps: true);

			CloudBlobClient client = account.CreateCloudBlobClient();
			return client;
		}

		public string UploadFiles(CloudBlobClient client, IEnumerable<string> files, string containerName)
		{
			ContainerName += containerName + "-" + Guid.NewGuid().ToString();
			CloudBlobContainer container = client.GetContainerReference(ContainerName);
			container.CreateIfNotExistsAsync().Wait();

			// create the stored policy we will use, with the relevant permissions and expiry time
			const int accessExpiryTime = 21;
			SharedAccessBlobPolicy storedPolicy = new SharedAccessBlobPolicy()
			{
				SharedAccessExpiryTime = DateTime.UtcNow.AddDays(accessExpiryTime),
				Permissions = SharedAccessBlobPermissions.Read |
							  SharedAccessBlobPermissions.Write |
							  SharedAccessBlobPermissions.List |
							  SharedAccessBlobPermissions.Delete,
			};

			SasUrl = container.Uri + container.GetSharedAccessSignature(storedPolicy);

			foreach (string file in files)
			{
				//think we can upload without extension, but for demo we uploaded PDFs
				CloudBlockBlob blob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".pdf");
				using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					blob.UploadFromStream(stream);
				}
			}

			return SasUrl;
		}

	}
}
