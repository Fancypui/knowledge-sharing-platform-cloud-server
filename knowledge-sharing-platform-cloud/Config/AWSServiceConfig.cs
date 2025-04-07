using Amazon.S3;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Services.Consumer;
using knowledge_sharing_platform_cloud.Services.impl;

namespace knowledge_sharing_platform_cloud.Config
{
    /**
     * AWS configuration file
     */
    public static class AWSServiceConfig
    {
        public static void ConfigureAwsServices(this IServiceCollection services, IConfiguration configuration)
        {
            /**
             * configure AWS options 
             */
            var awsOptions = new Amazon.Extensions.NETCore.Setup.AWSOptions
            {
                Region = Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"]),
                Credentials = new Amazon.Runtime.SessionAWSCredentials(
                configuration["AWS:AccessKey"],
                configuration["AWS:SecretKey"],
                configuration["AWS:SessionToken"])
            };
            services.AddDefaultAWSOptions(awsOptions);
            /**
             * s3 services
             */
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<S3Service>();
            ///**
            // * retrieve ARN value from appsettings
            // */
            var snsPushNotificationARN = configuration.GetValue<string>("AWS:SNSPushNotificationArn");
            var sqsPushNotificationQueueARN = configuration.GetValue<string>("AWS:SQSPushNotificationQueueARN");
            var sqsRedisChannelLeaderBoardARN = configuration.GetValue<string>("AWS:SQSRedisChannelLeaderBoardARN");
            //var sqsStripeWebhookQueueARN = configuration.GetValue<string>("AWS:SQSStripeWebhookQueueARN");

            var snsWebPushARN = configuration.GetValue<string>("AWS:SNSWebPush");
            ///**
            // * register SNS publisher, SQS publisher, and SQS handlers`
            // */
            services.AddAWSMessageBus(builder =>
            {

                /**
                 * register sns publisher (push notification topic)
                 */
                builder.AddSNSPublisher<PushNotificationDTO>(snsPushNotificationARN);
                /**
                 * register SNS publisher (Web Push Topic)
                 */
                builder.AddSNSPublisher<WebPushPaymentMsgDTO>(snsWebPushARN);
                /**
                 * register sqs publisher (queue to update channel leaderboard in redis)
                 */
                builder.AddSQSPublisher<ChannelLeaderboardDTO>(sqsRedisChannelLeaderBoardARN);
                /**
                 * register sqs queue (push notification queue) to poll messages
                 */
                builder.AddSQSPoller(sqsPushNotificationQueueARN, options =>
                {
                    // The maximum number of messages from this queue that the framework will process concurrently on this client
                    options.MaxNumberOfConcurrentMessages = 10;

                    // The duration each call to SQS will wait for new messages
                    options.WaitTimeSeconds = 20;
                });
                /**
                 * register sqs queue (event to update channel leaderboard in redis) to poll messages
                 */
                builder.AddSQSPoller(sqsRedisChannelLeaderBoardARN, options =>
                {
                    // The maximum number of messages from this queue that the framework will process concurrently on this client
                    options.MaxNumberOfConcurrentMessages = 10;

                    // The duration each call to SQS will wait for new messages
                    options.WaitTimeSeconds = 20;
                });
                /**
                 * register sqs queue (stripe webhook event) to poll messages
                 */
                //builder.AddSQSPoller(sqsStripeWebhookQueueARN, options =>
                //{
                //    options.MaxNumberOfConcurrentMessages = 10;
                //    options.WaitTimeSeconds = 20;
                //});
                //    /**
                //     * register SQS consumer
                //     */
                builder.AddMessageHandler<PushNotificationConsumer, PushNotificationDTO>();
                builder.AddMessageHandler<ChannelLeaderboardConsumer, ChannelLeaderboardDTO>();
                //builder.AddMessageHandler<StripeWebhookEventConsumer, StripeWebhookEventDTO>();
            });
        }
    }
}
