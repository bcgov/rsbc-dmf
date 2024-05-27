# CMS Adapter

This adapter connects the case management system.  The current implementation is specific to Microsoft Dynamics.

## Update OData client

You will need to update the OData client whenever the schema for MS Dynamics changes.

First, ensure that you have the "OData Connected Service 2022+ extension installed in Visual Studio.

In a browser, login to MS Dynamics and open developer tools in the browser.  Copy the "cookie" header value from the request headers.

Before updating, delete the "Reference.cs" file that contains the current client.  Otherwise you may see an error during the following steps because the file exists.

Within Visual Studio, right click the "Dynamics" connected service folder and select "Update OData Connected Service".

In the custom headers field, enter "Cookie: " followed by the contents of the cookie you captured earlier.

Click next and verify that you can see schema elements.

Click Finish and it should update the client.

# Dynamics Mapping

| Name              | Schema              | ViewModel      | Foreign Key
|-------------------|---------------------|----------------|----------------
| Document          | bcgov_documenturl   | LegacyDocument | dfp_DocumentTypeID, dfp_DocumentSubType
| Document Type     | dfp_submittaltype   | | dfp_submittaltypeid
| Document Sub-Type | dfp_documentsubtype | | dfp_DocumentSubType
| BringForward      | task                | Callback


# Dynamic Field Mapping


| Name              | Schema              | ViewModel      | Foreign Key
|-------------------------------------------------------------------------
| Document Type    | dfp_submittalType   | Document 
| DocumentTYpe.Name, Documenttype.SubmittalStatus, documentsubtype.name