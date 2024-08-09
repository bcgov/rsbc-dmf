## ONEHEALTH
The local, DEV, and TEST environments all use OneHealth for authentication and to load the endorsements with licences. 
The website is the same for all environments: test.healthprovideridentityportal.gov.bc.ca
Use this portal to add endorsements, enrol in DMFT, and to add licences

Use the PIDP0000X users, since the dmfw0000X users are not in a usable state and we have not heard back from OneHealth. Unfortunately, this means that the user data will be reset and does make testing difficult

## RUN

To run the solution in the root folder, you will need to add the following in multiple startup projects configuration:
- partner-portal
- Rsbc.Dmf.CaseManagement.Service
- Pssg.Dmf.IcbcAdapter
- Pssg.DocumentStorageAdapter

NOTE you will probably need to update the ICBC adapter port if it's 8080, to another port that is not in use already. Check Properties/launchSettings.json