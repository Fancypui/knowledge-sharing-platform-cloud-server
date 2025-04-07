
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using AWS.Messaging;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.PostReq;
using Newtonsoft.Json;
using System.Text.Json;

namespace knowledge_sharing_platform_cloud.Services.Consumer
{
    public class StripePaymentWebhookConsumer : BackgroundService
    {
        private readonly string _stripePaymentWebhookQueueArn;
        private readonly AmazonSQSClient _sqsClient;
        private readonly IServiceScopeFactory _scopeFactory;

        //private readonly IChannelService _channelService;

        public StripePaymentWebhookConsumer(IConfiguration config, IServiceScopeFactory scopeFactory)
        {

            _stripePaymentWebhookQueueArn = config["AWS:SQSStripeWebhookQueueARN"];
            _scopeFactory = scopeFactory;

            // Retrieve AWS credentials and session token
            var awsAccessKeyId = config["AWS:AccessKey"];
            var awsSecretAccessKey = config["AWS:SecretKey"];
            var awsSessionToken = config["AWS:SessionToken"];

            // Create AWS session credentials (for temporary credentials)
            var awsCredentials = new SessionAWSCredentials(
                awsAccessKeyId,           // your AWS Access Key
                awsSecretAccessKey,       // your AWS Secret Key
                awsSessionToken           // your session token
            );

            // Initialize the SQS client with the session credentials and region
            var sqsConfig = new AmazonSQSConfig
            {
                RegionEndpoint = RegionEndpoint.USEast1 // Replace with your desired region
            };

            // Create the SQS client
            _sqsClient = new AmazonSQSClient(awsCredentials, sqsConfig);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = _stripePaymentWebhookQueueArn,
                };
                var response = await _sqsClient.ReceiveMessageAsync(request);

                foreach (var message in response.Messages)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var _channelService = scope.ServiceProvider.GetRequiredService<IChannelService>();

                    StripeWebhookEventDTO stripeWebhookEventDTO = JsonConvert.DeserializeObject<StripeWebhookEventDTO>(message.Body);

                    if (stripeWebhookEventDTO.CheckoutSessionPaymentStatus == "paid")
                    {
                        try
                        {
                            await _channelService.JoinChannelSuccess(stripeWebhookEventDTO);
                            Console.WriteLine("join channel success");
                        }
                        catch (System.Exception ex)
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            await _channelService.JoinChannelFail(stripeWebhookEventDTO);
                            Console.WriteLine("join channel fail");
                        }
                        catch (System.Exception ex)
                        {

                        }
                    }

                }                
            }
        }
    }
}
