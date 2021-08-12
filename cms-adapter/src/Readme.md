# CMS Adapter

This adapter connects the case management system.  The current implementation is specific to Microsoft Dynamics.

## Update OData client

You will need to update the OData client whenever the schema for MS Dynamics changes.

First, ensure that you have the "OData Connected Service" extension installed in Visual Studio.

In a browser, login to MS Dynamics and open developer tools in the browser.  Get the "cookie" header value and set that aside.

Before updating, delete the "Reference.cs" file that contains the current client.  Otherwise you may see an error during the following steps because the file exists.

Within Visual Studio, right click the "Dynamics" connected service and select "Update".

In the special headers field, enter "cookie: " followed by the contents of the cookie you captured earlier.

Click next and verify that you can see schema elements.

Click Finish and it should update the client.



