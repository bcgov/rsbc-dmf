# Road Safety BC Driver Medical Fitness - Driver Portal
[Repository](https://github.com/bcgov/rsbc-dmf/tree/main/driver-portal)
[Link](https://roadsafetybc.gov.bc.ca/driver-portal) - Add subdomain dev or test for DEV and TEST environments e.g. https://dev.roadsafetybc.gov.bc.ca/driver-portal

## DOCUMENTATION
[Confluence](https://jag.gov.bc.ca/wiki/display/DFTP/%5BProject+Base%5D+-+Driver+Portal)
[Jira](https://jag.gov.bc.ca/jirarsi/secure/RapidBoard.jspa?rapidView=2503&projectKey=DFTDP)
[Wireframes](https://dmft.number41media.com/)

## PREREQUISITES
- Keycloak
- AWS S3
- OpenShift

## BUILD
- For all OS, you can use `dotnet build` from the root of the solution
- For Windows, you can use Visual Studio build
- For Mac, you can use JetBrains Rider build
- TODO add docker build steps to run "\Dockerfile" to build the docker image

## DEVELOP
- Visual Studio IDE 2022 (not tested with earlier versions)
- .NET 8.0 SDK

## RUN
To login, use dmfw0000X users, where X is a number from 1 to 20. The number part should be 5 digits padded with zeros e.g. dmfw00010

You will need to add the following in VS "Configure Startup Projects...":
- driver-portal
- Rsbc.Dmf.CaseManagement.Service
- Pssg.Dmf.IcbcAdapter
- Pssg.DocumentStorageAdapter

NOTE you will probably need to update the ICBC adapter port if it's 8080, to another port that is not in use already. Check Properties/launchSettings.json

To login, use your own IDIR account e.g. john.smith@gov.bc.ca

## DEPLOY
[Github Action - Api CI](https://github.com/bcgov/rsbc-dmf/actions/workflows/ci-driver-portal-api.yml)
[Github Action - Api CD](https://github.com/bcgov/rsbc-dmf/actions/workflows/cd-driver-portal-api.yml)
[Github Action - UI CI](https://github.com/bcgov/rsbc-dmf/actions/workflows/ci-driver-portal-ui.yml)
[Github Action - UI CD](https://github.com/bcgov/rsbc-dmf/actions/workflows/cd-driver-portal-ui.yml)

## DEBUG
Splunk: https://splunk.jag.gov.bc.ca/

## TEST
- See project "Rsbc.Dmf.PartnerPortal.Api.Tests\Rsbc.Dmf.PartnerPortal.Api.Tests.csproj" for unit tests
- [Swagger UI](http://localhost:8080/swagger/index.html)
- Postman see file "\DMFT.postman_collection.json"

## RESOURCES
[OpenShift Silver](https://oauth-openshift.apps.silver.devops.gov.bc.ca/oauth/authorize?client_id=console&redirect_uri=https%3A%2F%2Fconsole.apps.silver.devops.gov.bc.ca%2Fauth%2Fcallback&response_type=code&scope=user%3Afull&state=bd57c0b6)
[Keycloak](https://common-logon-test.hlth.gov.bc.ca/auth/)
AWS S3

### How to install shared libraries
 Refer to https://angular.dev/tools/libraries/creating-libraries for angular libraries.
1. Build the shared libraries first before we run any any portal 
    - npm run watch
2. To install shared library in the portal
    - npm install "C:\Users\ShruthiR.QSL\Documents\Development\rsbc-dmf\shared-portal-ui\dist\core-ui" (this is your dist folder of shared-portal-uI/dist shared libraries- copy your local path)
3. Update the CI pipleine to build the shared libraries first and then the portals(Refer to  any portal-UI CI pipleline)