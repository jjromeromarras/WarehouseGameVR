public class Usage 
{
    [Newtonsoft.Json.JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }
    [Newtonsoft.Json.JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    [Newtonsoft.Json.JsonProperty("prompt_cache_hit_tokens")]
    public int PromptCacheHitTokens { get; set; }
    [Newtonsoft.Json.JsonProperty("prompt_cache_miss_tokens")]
    public int PromptCacheMissTokens { get; set; }
    [Newtonsoft.Json.JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }

    [Newtonsoft.Json.JsonProperty("prompt_tokens_details")]
    public CompletionTokensDetails Details { get; set; }

    public class CompletionTokensDetails
    {
        [Newtonsoft.Json.JsonProperty("reasoning_tokens")]
        public int ReasoningTokens { get; set; }
        [Newtonsoft.Json.JsonProperty("cached_tokens")]
        public int CachedTokens { get; set; }
    }
}
