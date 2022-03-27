namespace Dtos.video
{

    public class YoutubeApiPageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class YoutubeApiVideoSnippetLocalized
    {
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }

    public class YoutubeApiVideoSnippetThumbnail
    {
        public string url { get; set; } = string.Empty;
        public int width { get; set; }
        public int height { get; set; }
    }

    public class YoutubeApiVideoSnippetThumbnails
    {
        public YoutubeApiVideoSnippetThumbnail? medium { get; set; }
        public YoutubeApiVideoSnippetThumbnail? high { get; set; }
    }

    public class YoutubeApiVideoSnippet
    {
        public string publishedAt { get; set; } = string.Empty;
        public string channelId { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string channelTitle { get; set; } = string.Empty;
        public string categoryId { get; set; } = string.Empty;
        public string liveBroadcastContent { get; set; } = string.Empty;
        public YoutubeApiVideoSnippetLocalized? localized { get; set; }
        public YoutubeApiVideoSnippetThumbnails? thumbnails { get; set; }
    }

    public class YoutubeApiVideo
    {
        public string kind { get; set; } = string.Empty;
        public string etag { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public YoutubeApiVideoSnippet? snippet { get; set; }
    }

    public class YoutubeApiResponseDto
    {
        public string kind { get; set; } = string.Empty;
        public string etag { get; set; } = string.Empty;
        public YoutubeApiPageInfo? pageInfo { get; set; }
        public List<YoutubeApiVideo> items { get; set; } = new List<YoutubeApiVideo>();
    }
}