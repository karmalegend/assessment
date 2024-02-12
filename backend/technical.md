## Technical Questions
### 1. How would you improve the API to make it production-ready (bug fixes, security, performance, etc.)?
* For bug fixes, we should implement a rigorous testing strategy that includes unit, integration, and end-to-end tests. And further expand the existing unittests. As mutation score and overall quality is lacking.
* Regarding security, we need to consider authorization and authentication. JWT tokens or OAuth can be used for this purpose. In addition to general code analyzers vulnerability scanners etc. 
* For performance, we should optimize our code and database queries. We can use performance profiling tools to identify bottlenecks. F.e. monitoring db query times checking hotpath optimizations heap allocations etc.
* Furthermore documentation should be written and the `Swagger` spec updated to reflect all potential responses.
* DTO's would also be a much wanted introduction.
* Lastly, we should consider using error & trace logging and error notifying tools for real-time problem detection.
* PS. one could also look into bundling most of the DB interaction in a transaction making it easier to roll back if somewhere along the line a businessrule is violated. it's currently designed with the possibility of something happening in mind and reducing the impact as much as possible. However it's worse than a transaction.

### 2. How would you make the API idempotent?
The Api is largely idempotent already given the small number of `POST` requests and the fact the one `PATCH` is implemented in an idempotent way. As for the `POST` method we could add an idempotency key as a request header. As largely described [here](https://datatracker.ietf.org/doc/draft-ietf-httpapi-idempotency-key-header/)

### 3. How would you approach the API authentication?
JSON Web Tokens (JWT) are a good fit for stateless API authentication. They're secure, and they can carry (NON-SENSITIVE) user data which can eliminate some need for database queries. OAuth2 is another very commonly used protocol, it would be appropriate if we need third-party integration or if you'd simply want to outsource most of the token headache.
To authenticate said tokens an API Management strategy could be used. Or the api itself could check the validity of said token.

### 4. What type of storage would you use for this service in production?
The choice of storage depends on the specific requirements and the nature of data we are working with. For a service like this, a SQL database like SQL Server or MySQL/MariaDB would likely be sufficient in most cases. But, if we need to store large amounts of non-relational or semi-structured data, a NoSQL database like MongoDB or DynamoDB could be considered as well.
This is all dependant on scale read/write load etc.

### 5. How would you deploy this API to production? Which infrastructure would you need for that (databases, messaging, etc)?
For deployment, I would propose to use Docker for containerization and Kubernetes for orchestration of those containers for easy scaling and management. For the infrastructure, along with the application server and the database server, we could use Nginx for ingress with APIM in front & some form of load balancer.
Something like RabbitMQ/kafka could be used if opted for an event-driven architecture.

### 6. How would you optimize the API endpoints to guarantee low latency under high load?
This could be achieved by implementing caching with tools like Redis. Database query optimization and using lightweight data structures will also help. And of course brute-force horizontal scaling.
