using System.Diagnostics.CodeAnalysis;

namespace ProcessadorVideo.CrossCutting.Configurations;


[ExcludeFromCodeCoverage]
public class AWSConfiguration
{
    public string Region { get; set; }
    public string ServiceUrl { get; set; }
    public string AccesKey { get; set; }
    public string Secret { get; set; }
    public string Token { get; set; }
    public string ConverterVideoParaImagemQueueUrl { get { return "converter-video-para-imagem"; } }
    public string ConversaoVideoParaImagemRealizadaQueueUrl { get { return "conversao-video-para-imagem-realizada"; } }
    public string ConversaoVideoParaImagemErroQueueUrl { get { return "erro-conversao-video-para-imagem"; } }


    public AWSConfiguration()
    {
        Region = GetEnvironmentVariableOrDefault("AWS_REGION", Region);
        Region = GetEnvironmentVariableOrDefault("AWS_ACCESS_KEY", AccesKey);
        Region = GetEnvironmentVariableOrDefault("AWS_SECRET", Secret);
        Region = GetEnvironmentVariableOrDefault("AWS_TOKEN", Token);
        Region = GetEnvironmentVariableOrDefault("AWS_SERVICE_URL", ServiceUrl);


    }


    private string GetEnvironmentVariableOrDefault(string variableName, string defaultValue = null)
    {
        string value = Environment.GetEnvironmentVariable(variableName);
        return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }
}
