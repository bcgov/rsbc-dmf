# ICBC Adapter

This adapter contains the integration with ICBC.  

## Dependencies

- Connection details for ICBC.  This includes a hostname and any required credentials for that endpoint.
- CMS Adapter is running and properly configured.

## API Documentation

Run the adapter and access /swagger in a browser to view the generated OpenAPI documentation.  Key features are also summarized below

### Driver History

The Driver History service is used to provide Driver History to MS Dynamics.  It can work with legacy XML data or JSON data, and it will normalize that to a JSON result.

### Update Medical Status

The Update Medical Status feature is used to send updates to ICBC.  It is meant to be triggered by an external Hangfire instance, and is only available via gRPC

### Medical Candidate Intake

The Medical Candidate Intake service is used to receive new Medical Candidates from ICBC.




