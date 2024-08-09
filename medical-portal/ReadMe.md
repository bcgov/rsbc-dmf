## ONEHEALTH
The local, DEV, and TEST environments all use OneHealth for authentication and to load the endorsements with licences. 
The website is the same for all environments: test.healthprovideridentityportal.gov.bc.ca
Use this portal to add endorsements, enrol in DMFT, and to add licences

Use the PIDP0000X users, since the dmfw0000X users are not in a usable state and we have not heard back from OneHealth. Unfortunately, this means that the user data will be reset and does make testing difficult
To add a licence to a PIDP user, use any licence type and use licence number that matches X e.g. PIDP00001 would have licence 1
To enrol the user in DMFT, click on the access link on top menu, click on "Get Access" link on the page, click on "Driver Medical Fitness", and then click on the "Enrol" button

HOW TO ADD ENDORSEMENTS
1. Login as user A
2. Navigate to endorsements
3. Enter the email of user B and request endorsement
4. Logout
5. Navigate to Mailhog
https://mailhog-test.healthprovideridentityportal.gov.bc.ca/
6. Open new email from user A and click "this link" and follow the instructions in the email
7. Login as user B after clicking the link
8. At the top of the page, you will see an pending endorsement link, click on the link
9. Under incoming requests, you will see a request from user A, click Approve
10. Logout
11. Navigate to Mailhog
12. Open new email from system and click the link and follow the instructions in the email
13. Login as user A
14. Click on the pending endorsement request link
15. Under incoming requests, you will see an request from user B, click Approve
16. You will receive a new email and also see a new "Working relationship" for both users

Contacts:
Nick Mailhot
Sekhon, Khushwinder <Khushwinder.Sekhon@gov.bc.ca> - For updating the dmfw0000X users, which were suppose to be persisted for our testing
