using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoIndexing.Agents
{

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Summarizedinsights
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string name { get; set; }
		public string id { get; set; }
		public string privacyMode { get; set; }
		public Duration duration { get; set; }
		public string thumbnailVideoId { get; set; }
		public string thumbnailId { get; set; }
		public Face[] faces { get; set; }
		public Keyword[] keywords { get; set; }
		public Sentiment[] sentiments { get; set; }
		public Emotion[] emotions { get; set; }
		public object[] audioEffects { get; set; }
		public Label[] labels { get; set; }
		public object[] framePatterns { get; set; }
		public object[] brands { get; set; }
		public object[] namedLocations { get; set; }
		public object[] namedPeople { get; set; }
		public Statistics statistics { get; set; }
		public Topic[] topics { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Duration
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string time { get; set; }
		public float seconds { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Statistics
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int correspondenceCount { get; set; }
		public Speakertalktolistenratio speakerTalkToListenRatio { get; set; }
		public Speakerlongestmonolog speakerLongestMonolog { get; set; }
		public Speakernumberoffragments speakerNumberOfFragments { get; set; }
		public Speakerwordcount speakerWordCount { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Speakertalktolistenratio
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public float _1 { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Speakerlongestmonolog
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int _1 { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Speakernumberoffragments
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int _1 { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Speakerwordcount
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int _1 { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Face
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string videoId { get; set; }
		public string referenceId { get; set; }
		public string referenceType { get; set; }
		public float confidence { get; set; }
		public string description { get; set; }
		public string title { get; set; }
		public string thumbnailId { get; set; }
		public float seenDuration { get; set; }
		public float seenDurationRatio { get; set; }
		public int id { get; set; }
		public string name { get; set; }
		public Appearance[] appearances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Appearance
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string startTime { get; set; }
		public string endTime { get; set; }
		public float startSeconds { get; set; }
		public float endSeconds { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Keyword
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public bool isTranscript { get; set; }
		public int id { get; set; }
		public string name { get; set; }
		public Appearance[] appearances { get; set; }
	}


#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Sentiment
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string sentimentKey { get; set; }
		public float seenDurationRatio { get; set; }
		public Appearance[] appearances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Emotion
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string type { get; set; }
		public float seenDurationRatio { get; set; }
		public Appearance[] appearances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Label
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public string name { get; set; }
		public Appearance[] appearances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Topic
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public object referenceUrl { get; set; }
		public string iptcName { get; set; }
		public string iabName { get; set; }
		public float confidence { get; set; }
		public int id { get; set; }
		public string name { get; set; }
		public Appearance[] appearances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Video
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string accountId { get; set; }
		public string id { get; set; }
		public string state { get; set; }
		public string moderationState { get; set; }
		public string reviewState { get; set; }
		public string privacyMode { get; set; }
		public string processingProgress { get; set; }
		public string failureCode { get; set; }
		public string failureMessage { get; set; }
		public object externalId { get; set; }
		public object externalUrl { get; set; }
		public object metadata { get; set; }
		public Insights insights { get; set; }
		public string thumbnailId { get; set; }
		public bool detectSourceLanguage { get; set; }
		public string sourceLanguage { get; set; }
		public string language { get; set; }
		public string indexingPreset { get; set; }
		public string linguisticModelId { get; set; }
		public string personModelId { get; set; }
		public string animationModelId { get; set; }
		public bool isAdult { get; set; }
		public string publishedUrl { get; set; }
		public object publishedProxyUrl { get; set; }
		public string viewToken { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Insights
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string version { get; set; }
		public string duration { get; set; }
		public string sourceLanguage { get; set; }
		public string language { get; set; }
		public Transcript[] transcript { get; set; }
		public Ocr[] ocr { get; set; }
		public Keyword[] keywords { get; set; }
		public Topic[] topics { get; set; }
		public Face[] faces { get; set; }
		public Label[] labels { get; set; }
		public Scene[] scenes { get; set; }
		public Shot[] shots { get; set; }
		public Sentiment[] sentiments { get; set; }
		public Emotion[] emotions { get; set; }
		public Block[] blocks { get; set; }
		public Speaker[] speakers { get; set; }
		public Textualcontentmoderation textualContentModeration { get; set; }
		public Statistics statistics { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Textualcontentmoderation
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public int bannedWordsCount { get; set; }
		public int bannedWordsRatio { get; set; }
		public Instance[] instances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Transcript
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public string text { get; set; }
		public float confidence { get; set; }
		public int speakerId { get; set; }
		public string language { get; set; }
		public Instance[] instances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Ocr
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public string text { get; set; }
		public float confidence { get; set; }
		public int left { get; set; }
		public int top { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public string language { get; set; }
		public Instance[] instances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Thumbnail
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string id { get; set; }
		public string fileName { get; set; }
		public Instance[] instances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Scene
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public Instance[] instances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Shot
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public Keyframe[] keyFrames { get; set; }
		public Instance[] instances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Keyframe
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public Instance[] instances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Instance
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public float confidence { get; set; }
		public string adjustedStart { get; set; }
		public string adjustedEnd { get; set; }
		public string start { get; set; }
		public string end { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Block
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public Instance[] instances { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Speaker
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public int id { get; set; }
		public string name { get; set; }
		public Instance[] instances { get; set; }
	}


#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Videosrange
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string videoId { get; set; }
		public Range range { get; set; }
	}

#pragma warning disable RG0001 // Class Matching File Name
#pragma warning disable RG2002 // Class Count
	public class Range
#pragma warning restore RG2002 // Class Count
#pragma warning restore RG0001 // Class Matching File Name
	{
		public string start { get; set; }
		public string end { get; set; }
	}

}
