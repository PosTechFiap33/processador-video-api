using System.Diagnostics.CodeAnalysis;

namespace ProcessadorVideo.CrossCutting.Configurations;


[ExcludeFromCodeCoverage]
public class AWSConfiguration
{
    public string Region { get; set; }
    public string ServiceUrl { get; set; }
    public string AwsQueueUrl { get; set; }
    public string ConverterVideoParaImagemQueueUrl { get { return $"{AwsQueueUrl}/converter-video-para-imagem"; } }
    public string ConversaoVideoParaImagemRealizadaQueueUrl { get { return $"{AwsQueueUrl}/conversao-video-para-imagem-realizada"; } }
    public string ConversaoVideoParaImagemErroQueueUrl { get { return $"{AwsQueueUrl}/erro-conversao-video-para-imagem"; } }


    public AWSConfiguration()
    {
        Region = GetEnvironmentVariableOrDefault("AWS_REGION", Region);
        ServiceUrl = GetEnvironmentVariableOrDefault("AWS_SERVICE_URL", ServiceUrl);
        AwsQueueUrl = GetEnvironmentVariableOrDefault("AWS_QUEUE", AwsQueueUrl);
    }


    private string GetEnvironmentVariableOrDefault(string variableName, string defaultValue = null)
    {
        string value = Environment.GetEnvironmentVariable(variableName);
        return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }
}
