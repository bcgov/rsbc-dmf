## RUN

To run the solution in the root folder, you will need to add the following in multiple startup projects configuration:
- partner-portal
- Rsbc.Dmf.CaseManagement.Service
- Pssg.Dmf.IcbcAdapter
- Pssg.DocumentStorageAdapter

NOTE you will probably need to update the ICBC adapter port if it's 8080, to another port that is not in use already. Check Properties/launchSettings.json