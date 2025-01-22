using System.Diagnostics.CodeAnalysis;
using Amazon.Runtime;
using ProcessadorVideo.CrossCutting.Configurations;

namespace ProcessadorVideo.CrossCutting.Factories;

[ExcludeFromCodeCoverage]
public static class AwsCredentialsFactory
{
    public static BasicAWSCredentials CreateCredentials(this ClientConfig config, AWSConfiguration configuration)
    {
        var credentials = new BasicAWSCredentials(configuration.AccessKey, configuration.SecretKey);

        if (!string.IsNullOrEmpty(configuration.ServiceUrl))
            config.ServiceURL = configuration.ServiceUrl;

        return credentials;
    }
}