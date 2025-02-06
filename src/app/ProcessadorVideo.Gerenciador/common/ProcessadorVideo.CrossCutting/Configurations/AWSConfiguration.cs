using System.Diagnostics.CodeAnalysis;

namespace ProcessadorVideo.CrossCutting.Configurations
{
    [ExcludeFromCodeCoverage]
    public class AWSConfiguration
    {
        private string _region;
        private string _serviceUrl;
        private string _accesKey;
        private string _secret;
        private string _token;

        public string Region { get { return GetEnvironmentVariableOrDefault("AWS_REGION", _region); } set { _region = value; } }
        public string ServiceUrl { get { return GetEnvironmentVariableOrDefault("AWS_SERVICE_URL", _serviceUrl); } set { _serviceUrl = value; } }
        public string AccesKey { get { return GetEnvironmentVariableOrDefault("AWS_ACCESS_KEY", _accesKey); } set { _accesKey = value; } }
        public string Secret { get { return GetEnvironmentVariableOrDefault("AWS_SECRET", _secret); } set { _secret = value; } }
        public string Token { get { return GetEnvironmentVariableOrDefault("AWS_TOKEN", _token); } set { _token = value; } }

        public string ConverterVideoParaImagemQueueUrl { get { return "converter-video-para-imagem"; } }
        public string ConversaoVideoParaImagemRealizadaQueueUrl { get { return "conversao-video-para-imagem-realizada"; } }
        public string ConversaoVideoParaImagemErroQueueUrl { get { return "erro-conversao-video-para-imagem"; } }

        private string GetEnvironmentVariableOrDefault(string variableName, string defaultValue = null)
        {
            string value = Environment.GetEnvironmentVariable(variableName);
            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }
    }
}
