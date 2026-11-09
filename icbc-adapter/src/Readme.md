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

### DMER Document Download

`POST /dmers/download` accepts a JSON body with a required `fileUrl` storage path.
It uses the existing document storage adapter gRPC client's `DownloadFile`
operation, passing the path as `ServerRelativeUrl`. The response is a downloadable
file containing the returned bytes, with a filename and MIME type inferred from
the path (or `application/octet-stream` for unrecognized extensions).

The endpoint uses the ICBC adapter's existing authorization and document storage
client registration (`DOCUMENT_STORAGE_ADAPTER_URI` and
`DOCUMENT_STORAGE_ADAPTER_JWT_SECRET`). Missing or blank paths return 400.
Storage failures return 500; gRPC unavailability returns 503, timeouts return 504,
and other gRPC errors return 502, with Problem Details responses.
Example requests are in
[Pssg.Dmf.IcbcAdapter.http](Pssg.Dmf.IcbcAdapter/Pssg.Dmf.IcbcAdapter.http).


