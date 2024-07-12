## OVERVIEW
Road Safety BC Driver Medical Fitness - Medical Practitioner Portal
Repository: https://github.com/bcgov/rsbc-dmf/tree/main/medical-portal/src/API
Authors: Quartech

## DOCUMENTATION
Jira: https://jag.gov.bc.ca/wiki/display/DFTP/%5BProject+Base%5D+-+Practitioner+Portal+and+eDMER

## PREREQUISITES
- Keycloak
- Chefs
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
Run solution "\medical-portal.sln" to run all projects needed for medical portal or run individual projects as needed

## DEPLOY
Github Action: https://github.com/bcgov/rsbc-dmf/actions/workflows/cd-medical-portal-api.yml

## DEBUG
Splunk: https://splunk.jag.gov.bc.ca/

## TEST
- See project "\RSBC.Tests.Unit.DMF.MedicalPortal.API\RSBC.Tests.Unit.DMF.MedicalPortal.API.csproj" for unit tests
- Swagger UI http://localhost:5000/swagger/index.html
- Postman see file "\DMFT.postman_collection.json"

## RESOURCES
OpenShift Silver: https://oauth-openshift.apps.silver.devops.gov.bc.ca/oauth/authorize?client_id=console&redirect_uri=https%3A%2F%2Fconsole.apps.silver.devops.gov.bc.ca%2Fauth%2Fcallback&response_type=code&scope=user%3Afull&state=bd57c0b6
Keycloak: https://test.healthprovideridentityportal.gov.bc.ca/
AWS S3
Chefs: https://submit.digital.gov.bc.ca/app/
Chefs test eDMER form: https://submit.digital.gov.bc.ca/app/form/manage?f=5383fc89-b219-49a2-924c-251cd1557eb8