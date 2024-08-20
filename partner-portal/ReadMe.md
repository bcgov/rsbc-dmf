# Road Safety BC Driver Medical Fitness - Partner Portal
[Repository](https://github.com/bcgov/rsbc-dmf/tree/main/partner-portal)

## DOCUMENTATION
[Confluence](https://jag.gov.bc.ca/wiki/display/DFTP/%5BProject+Base%5D+-+Partners+Portal)
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
Run solution "\partner-portal.sln" to run all projects needed for partner portal or run individual projects as needed
To run the frontend, cd to folder /partner-portal/src/UI and run `npm run start`

You will need to add the following in VS "Configure Startup Projects...":
- Rsbc.Dmf.CaseManagement.Service
- Pssg.Dmf.IcbcAdapter
- Pssg.DocumentStorageAdapter
- partner-portal

NOTE you will probably need to update the ICBC adapter port if it's 8080, to another port that is not in use already. Check Properties/launchSettings.json

To login, use your own IDIR account e.g. john.smith@gov.bc.ca

## DEPLOY
[Github Action - Api CI](https://github.com/bcgov/rsbc-dmf/actions/workflows/ci-partner-portal-api.yml)
[Github Action - Api CD](https://github.com/bcgov/rsbc-dmf/actions/workflows/cd-partner-portal-api.yml)
[Github Action - UI CI](https://github.com/bcgov/rsbc-dmf/actions/workflows/ci-partner-portal-ui.yml) (does not exist yet)
[Github Action - UI CD](https://github.com/bcgov/rsbc-dmf/actions/workflows/cd-partner-portal-ui.yml)

## DEBUG
[Splunk](https://splunk.jag.gov.bc.ca/)

## TEST
- See project "Rsbc.Dmf.PartnerPortal.Api.Tests\Rsbc.Dmf.PartnerPortal.Api.Tests.csproj" for unit tests
- [Swagger UI](http://localhost:8080/swagger/index.html)
- Postman see file "\DMFT.postman_collection.json"

## RESOURCES
[OpenShift Silver](https://oauth-openshift.apps.silver.devops.gov.bc.ca/oauth/authorize?client_id=console&redirect_uri=https%3A%2F%2Fconsole.apps.silver.devops.gov.bc.ca%2Fauth%2Fcallback&response_type=code&scope=user%3Afull&state=bd57c0b6)
[Keycloak](https://test.healthprovideridentityportal.gov.bc.ca/)
AWS S3
