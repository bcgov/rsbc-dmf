REM OAUTH SERVER

oc process -f ..\oauth-server\openshift\templates\oauth-server.template.yml --param-file ..\..\..\params\oauth-server.test.params | oc apply -f -

REM DOCUMENT STORAGE Adapter

oc process -f ..\document-storage-adapter\openshift\templates\document-storage-adapter.template.yml --param-file ..\..\..\params\document-storage-adapter.test.params | oc apply -f -

REM CMS ADAPTER

oc process -f ..\cms-adapter\openshift\templates\cms-adapter.template.yml --param-file ..\..\..\params\cms-adapter.test.params | oc apply -f -

REM DOCUMENT TRIAGE SERVICE

oc process -f ..\document-triage-service\openshift\templates\document-triage-service.template.yml --param-file ..\..\..\params\document-triage-service.test.params | oc apply -f -

REM PHSA Adapter

oc process -f ..\phsa-adapter\openshift\templates\phsa-adapter.template.yml --param-file ..\..\..\params\phsa-adapter.test.params | oc apply -f -

REM LANDING PAGE

oc process -f ..\landing-page\openshift\templates\landing-page.template.yml --param-file ..\..\..\params\landing-page.test.params | oc apply -f -

REM MEDICAL PORTAL

oc process -f ..\doctors-portal\openshift\templates\doctors-portal.template.yml --param-file ..\..\..\params\doctors-portal.test.params | oc apply -f -