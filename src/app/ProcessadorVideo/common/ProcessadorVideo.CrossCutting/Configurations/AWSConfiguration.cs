using System.Diagnostics.CodeAnalysis;

namespace ProcessadorVideo.CrossCutting.Configurations;


[ExcludeFromCodeCoverage]
public class AWSConfiguration
{
    public string Region { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string ServiceUrl { get; set; }
    public string AwsQueueUrl { get; set; }
    public string ProcessarVideoQueueUrl { get { return $"{AwsQueueUrl}/converter-video-para-imagem"; } }

    public AWSConfiguration()
    {
        Region = GetEnvironmentVariableOrDefault("AWS_REGION", Region);
        AccessKey = GetEnvironmentVariableOrDefault("AWS_ACCESS_KEY", AccessKey);
        SecretKey = GetEnvironmentVariableOrDefault("AWS_SECRET_KEY", SecretKey);
        ServiceUrl = GetEnvironmentVariableOrDefault("AWS_SERVICE_URL", ServiceUrl);
        AwsQueueUrl = GetEnvironmentVariableOrDefault("AWS_CONVERTER_VIDEO_IMAGEM_QUEUE", AwsQueueUrl);
    }
    

    private string GetEnvironmentVariableOrDefault(string variableName, string defaultValue = null)
    {
        string value = Environment.GetEnvironmentVariable(variableName);
        return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }
}
