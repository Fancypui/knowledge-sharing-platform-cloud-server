**Project Description**

A full-stack, cloud-native web application that empowers users to create and participate in specialized knowledge channels. Designed for scalability, real-time performance, and rich user interaction, this platform enables structured information exchange among communities with similar interests.

Built with C# .NET Web API on the backend and Next.js on the frontend, the system leverages AWS cloud services to handle deployments, media storage, asynchronous messaging, payment processing, and serverless computation.

---

**System Architecture**

![image](https://github.com/user-attachments/assets/4ddde5a5-e6fb-47d5-8d05-e679798a4879)

---

**Core Components**

**Backend (C# .NET Web API):**  
Backend services are containerized using Docker, pushed to AWS Elastic Container Registry (ECR), and deployed via AWS Elastic Container Service (ECS) on AWS Fargate. Tasks run in multiple Availability Zones for high availability and fault tolerance.

**Database Layer:**  
Uses Amazon RDS (SQL Server) to store persistent data such as users, posts, channels, and subscription/payment records.

**Caching Layer:**  
Redis is used as an in-memory data store to cache frequently accessed data such as channel metadata, leaderboard results, and user session information—significantly reducing response times and database load.

---

**AWS Integration**

**Deployment & Orchestration**  
- **AWS ECS + Fargate**: Manages and runs the containerized backend services in a serverless fashion.  
- **AWS ECR**: Stores Docker images of the backend for ECS deployments.  
- **Multi-AZ Fargate Tasks**: Ensures horizontal scaling and high availability by running tasks across multiple AWS availability zones.

**Image Upload & Optimization**  
- User-uploaded images are stored in an S3 bucket (Unresized).  
- An Image Resize Lambda function is triggered to create optimized versions, stored in a second S3 bucket (Resized).  
- Frontend retrieves images from the optimized bucket for performance.

**Asynchronous Processing**  
- Amazon SQS Queues handle background jobs like channel leaderboard updates and payment processing.  
- Amazon SNS pushes event notifications for successful checkouts or other key events.

**Payment Integration**  
- Integrated with Stripe for payment processing.  
- AWS EventBridge captures Stripe events, triggering a Webhook Lambda to process and validate transactions in real-time.

**Web Push Notifications**  
- A dedicated Lambda function handles dispatching of web push notifications to users for real-time engagement.

---

**Key Features**

- **Knowledge Channels**: Users can create, join, and contribute to topic-specific knowledge-sharing communities.
- **Rich Post Support**: Users can publish multimedia posts (images), stored and optimized via AWS S3 and Lambda.
- **Gamified Leaderboards**: Leaderboards powered by SQS and Redis cache are updated in near real-time to encourage user contribution.
- **Real-Time Web Push Notifications**: Stay updated with new content and interactions through browser push.
- **Stripe Integration**: Secure payment handling for channel access.
- **Serverless & Scalable**: Deployed using AWS ECS Fargate, Lambda functions, and managed services for effortless scaling.
- **Event-Driven Architecture**: Decoupled design powered by EventBridge, SNS, and SQS for resilient operations.

---

