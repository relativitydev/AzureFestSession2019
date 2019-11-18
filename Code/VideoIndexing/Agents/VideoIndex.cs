using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VideoIndexing.Agents
{
	public class VideoIndex
	{
		private string _videoPath;
		private string _videoName;
		private string _controlNumber;
		private string _indexerApiUrl;
		private string _indexerApiKey;
		private string _videoID;
		private string _indexerLocation;
		private Relativity.API.IAPILog _logger;
		private Relativity.API.IAgentHelper _helper;

		public VideoIndex(string videoPath, string videoName, string controlNumber, string indexerApiUrl, string indexerApiKey, Relativity.API.IAPILog logger, Relativity.API.IAgentHelper helper)
		{
			_videoPath = videoPath;
			_videoName = videoName;
			_controlNumber = controlNumber;
			_indexerApiUrl = indexerApiUrl;
			_indexerApiKey = indexerApiKey;
			_logger = logger;
			_helper = helper;
		}

		public async Task<VideoIndexResult> Sample()
		{
			VideoIndexResult videoIndexResult = new VideoIndexResult();
			try
			{
				//get these values from secret store
				_indexerApiUrl = "https://api.videoindexer.ai";
				_indexerApiKey = ""; //Add your own credentials here
				_indexerLocation = "trial";

				string transcript;

				System.Net.ServicePointManager.SecurityProtocol =
					System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls12;

				// create the http client
				HttpClientHandler handler = new HttpClientHandler();
				handler.AllowAutoRedirect = false;
				HttpClient client = new HttpClient(handler);
				client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _indexerApiKey);

				// obtain account information and access token
				string queryParams = CreateQueryString(
					new Dictionary<string, string>()
				{
					{"generateAccessTokens", "true"},
					{"allowEdit", "true"},
				});
				HttpResponseMessage result = await client.GetAsync($"{_indexerApiUrl}/auth/{_indexerLocation}/Accounts?{queryParams}");
				string json = await result.Content.ReadAsStringAsync();
				AccountContractSlim[] accounts = JsonConvert.DeserializeObject<AccountContractSlim[]>(json);
				// take the relevant account, here we simply take the first
				AccountContractSlim accountInfo = accounts.First();

				// we will use the access token from here on, no need for the apim key
				client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");

				// upload a video
				MultipartFormDataContent content = new MultipartFormDataContent();

				queryParams = CreateQueryString(
					new Dictionary<string, string>()
				{
					{"accessToken", accountInfo.AccessToken},
					{"name", string.Format("{0} - {1}",_controlNumber,_videoName)},
					{"description", "video_description"},
					{"privacy", "private"},
					{"partition", "partition"},
				});

				FileStream video = File.OpenRead(_videoPath);
				byte[] buffer = new byte[video.Length];
				video.Read(buffer, 0, buffer.Length);
				content.Add(new ByteArrayContent(buffer));

				HttpResponseMessage uploadRequestResult = await client.PostAsync($"{_indexerApiUrl}/{accountInfo.Location}/Accounts/{accountInfo.Id}/Videos?{queryParams}", content);
				string uploadResult = await uploadRequestResult.Content.ReadAsStringAsync();

				// get the video ID from the upload result
				_videoID = JsonConvert.DeserializeObject<dynamic>(uploadResult)["id"];

				// wait for the video index to finish
				const int threadSleep = 10000;
				while (true)
				{
					await Task.Delay(threadSleep);

					queryParams = CreateQueryString(
						new Dictionary<string, string>()
					{
						{"accessToken", accountInfo.AccessToken},
						{"language", "English"},
					});

					HttpResponseMessage videoGetIndexRequestResult = await client.GetAsync($"{_indexerApiUrl}/{accountInfo.Location}/Accounts/{accountInfo.Id}/Videos/{_videoID}/Index?{queryParams}");
					string videoGetIndexResult = await videoGetIndexRequestResult.Content.ReadAsStringAsync();

					string processingState = JsonConvert.DeserializeObject<dynamic>(videoGetIndexResult)["state"];

					// job is finished
					if (processingState != "Uploaded" && processingState != "Processing")
					{
						transcript = CreateTranscript(videoGetIndexResult);
						break;
					}
				}

				//set results
				videoIndexResult.VideoName = _videoName;
				videoIndexResult.ControlNumber = _controlNumber;
				videoIndexResult.VideoID = _videoID;
				videoIndexResult.Transcript = transcript;
			}
			catch (Exception ex)
			{
				LogError(ex);
			}

			return videoIndexResult;
		}

		private string CreateTranscript(string json)
		{
			StringBuilder sb = new StringBuilder();
			try
			{
				Rootobject root = JsonConvert.DeserializeObject<Rootobject>(json);
				Video[] videos = root.videos;
				foreach (Video video in videos)
				{
					Insights insights = video.insights;
					foreach (Transcript transcript in insights.transcript)
					{
						string utterance = transcript.text;
						string starttime = transcript.instances[0].start;
						string endtime = transcript.instances[0].end;
						sb.AppendLine(string.Format("{0} - {1} | {2}", starttime.ToString(), endtime.ToString(), utterance.ToString()));
					}
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}

			//return json;
			return sb.ToString();
		}

		private string CreateQueryString(IDictionary<string, string> parameters)
		{
			System.Collections.Specialized.NameValueCollection queryParameters = HttpUtility.ParseQueryString(string.Empty);
			try
			{
				foreach (KeyValuePair<string, string> parameter in parameters)
				{
					queryParameters[parameter.Key] = parameter.Value;
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
			return queryParameters.ToString();
		}

		private void LogError(Exception ex, [CallerMemberName] string caller = null)
		{
			_logger = _logger ?? _helper.GetLoggerFactory().GetLogger().ForContext<VideoIndex>();
			_logger.LogError(ex, "{caller} Error: {message}", caller, ex?.Message);
		}
	}
}
