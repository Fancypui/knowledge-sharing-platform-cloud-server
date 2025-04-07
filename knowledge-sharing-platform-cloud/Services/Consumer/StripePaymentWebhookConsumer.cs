
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace knowledge_sharing_platform_cloud.Services.Consumer
{
    public class StripePaymentWebhookConsumer : BackgroundService
    {
        private readonly string _stripePaymentWebhookQueueArn;
        private readonly AmazonSQSClient _sqsClient;
        public StripePaymentWebhookConsumer(IConfiguration config)
        {

            _stripePaymentWebhookQueueArn = config["AWS:SQSStripeWebhookQueueARN"];
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
            _sqsClient = new AmazonSQSClient(awsCredentials);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("hello");
            while (!stoppingToken.IsCancellationRequested)
            {
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = _stripePaymentWebhookQueueArn,
                };
                var response = await _sqsClient.ReceiveMessageAsync(request);
                foreach (var message in response.Messages)
                {
                    Console.WriteLine("New message received");
                    Console.WriteLine(message.Body);
                }
            }
        }
    }
}
