using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace FeedLens.Helpers
{
    public static class AwsParameterStoreHelper
    {
        public static async Task<Dictionary<string, string>> LoadParametersAsync(
            string accessKey,
            string secretKey,
            string region,
            List<string> parameterNames)
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonSimpleSystemsManagementConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
            };

            using var client = new AmazonSimpleSystemsManagementClient(credentials, config);

            var request = new GetParametersRequest
            {
                Names = parameterNames,
                WithDecryption = true
            };

            var response = await client.GetParametersAsync(request);

            return response.Parameters.ToDictionary(p => p.Name, p => p.Value);
        }
    }
}
