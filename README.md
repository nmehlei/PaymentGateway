# Overview
This is an example project of a Payment Gateway API that allows merchants to offer a way for shoppers to pay for a product.
The project is implemented as an ASP.NET Core project with .NET Core 3.1.

The solution contains the following projects (excluding unit test projects):
* PaymentGateway.API, containing the API service
* PaymentGateway.Domain, containing the domain model
* PaymentGateway.Persistence, containing the persistence layer
* PaymentGateway.Shared, including an API client and shared models/DTOs

# Features

## Containerization

For simplified portability, the API was implemented with **Docker** container and Docker-compose orchestration support. The orchestration includes two services, the API itself and an SQL server container, both Linux-based.

## API specification

In the subdirectory *API-Spec* a file named *swagger.json* includes **Swagger/OpenAPI**-compliant (version 3) API specification.
After starting the application, an online UI to display this specification can be accessed under:

> http://localhost:7000/swagger/index.html

## Postman requests

The subdirectory **Postman** contains request templates that were prepared for importing into Postman to simplify the manual usage of the API.

## API client

The _PaymentGatewayClient_ class and corresponding _IPaymentGatewayClient_ interface can be used to connect from a .NET-based Application to the API. 
The client is based on HttpClient and can be configured with the target base address, API key and, if necessary, a custom timeout.
Possible improvement for optimized resiliency would be adaptive retry behavior, for example with the help of [Polly](https://github.com/App-vNext/Polly).

## Authentication
In this project the authentication is handled with an API key per Merchant that is transmitted as a header value 
as part of each request to the API. This might be extended with an access token that is requested by the Merchant 
in his first request with the help of the API key and afterwards the usage of said access token instead of the API key for all 
subsequent requests until a calculated expiration date.

Authentication was kept simple on purpose because in a Production system in a Money-centric online-accessible solution it would, 
depending on the actual Production requirements, probably be the wisest decision to use an already existing Identity framework or system.

## Domain-Driven Design
The application was developed and implemented with an emphasis on a clear and concise DDD-based domain model,
with the provided requirement document acting on behalf of actual discussions between developer and domain expert.

Two aggregates were created: 
* Payment
* Merchant

Both aggregates have matching repositories based on the Repository pattern.

Additionally, a domain service was implemented for Bank integration. This is based on the interface _IBankConnectorDomainService_ with the currently only implementation _MockingTestBankConnectorDomainService_. This implementation simulates a Bank, randomizes the successfulness of the Payment order and returns this success/fail state plus additional data to the domain model.

## Data storage / Persistence
The domain model can be persisted with the help of Entity Framework Core.
This logic was moved to a separate project to abstract away all persistence concerns from the domain model.

Three merchants are automatically included as Seed data.
Concurrency is handled with the help of a timestamp value that is updated on every database change operation. Primary keys are GUID-based for both payments and merchants with sequential GUID generation for improved performance.

## Instrumentation, metrics and logging

Instrumentation, metrics and logs are automatically captured and recorded because of an integration with Application Insights. The corresponding _instrumentation key_ in the appsettings.json (or configured as an environment variable in the _docker-compose.yml_ file).

## Unit tests
All unit tests were implemented with the help of the MSTest testing framework.

# Usage

The solution was prepared as a complete self-contained ready-to-use environment due to the usage of two docker containers. The source code repository contains a PowerShell-based script to build and run the Docker containers. Just execute the following line:

> run.ps1

Afterwards, the API will be accessible under http://localhost:7000/. Please use the provided Postman requests to try out the API.

# Notes regarding global code decisions

## HTTPS/TLS termination via reverse proxy

The API inside the docker container offers an HTTP port (and not HTTPS) because a reverse proxy (like nginx, Traefik, Azure Application Gateway, etc.) is assumed to be in front of the API to handle HTTPS/TLS termination and offloading. This allows the container and API to be decoupled from the complexity of SSL certificates.

## Sync process & async processing

The API interface follows a synchronous process, as in calls return a result without any need for polling or subscribing, but processes 
those requests with asynchronous operations for increased scalability.

## "Async" method suffixes

I opted to omit the “Async” suffix from Aggregate method names to not blur the Ubiquitous Language of the domain model with technical implementation details.
In all other cases, the best practice from Microsoft (see: https://docs.microsoft.com/en-us/dotnet/csharp/async) was followed and the Suffix was added.

## Domain modeling: Payment vs Payment Request

The merchant requests that a payment shall happen, though a payment is defined as money actually being transferred. As this can fail, one could argue (and this would get more clear very fast while discussing this issue with a domain expert) that the main aggregate is not a payment and instead should be a payment request. As the requirement document in most occurences defines this as a payment it was opted to use payment though in actual discussions with a domain expert this would be the first topic that would have been discussed.

# Improvement ideas

## Idempotency

While the _Get Payment_ operation is idempotent by definition, the _Create Payment_ operation is not. This can lead to duplicate payment requests (and possibly, duplicate payments) if, for example, operations are sent twice by the customer. This could and should be improved. 

There are several possibilities for how this could be implemented. One variant, that could be implemented without a lot of changes, would be to add a new field named _IdempotencyKey_ in the _PaymentAggregate_ and the _PostPaymentRequestModel_ and create a unique Index in the persistence layer on the tuple of _MerchantId_ and the new _IdempotencyKey_. Assuming an equal _IdempotencyKey_ for all of the duplicate requests (which would be the responsibility of the merchant/API-client), every subsequent duplicate payment would fail (which could then be ignored and returned to the caller with a corresponding status code).

The benefit of this solution would be the fast and straight-forward implementation. The downside would be that this would happen inside the persistence layer and not in the domain model.

## Additional query interface for all payments of a merchant

An interface to query all (or a subset) of the payments of a merchant might be useful,
since if, should the merchant not know all payment IDs for its payments, this could be used
by the merchant during its reconciliation processes to find those cases.

## Further validation

The request validation currently focuses on the Merchant ID and API Key. A useful addition to that would be validation of the request models, for example via Fluent Data Annotations.
