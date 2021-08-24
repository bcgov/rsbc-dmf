REM PHSA Adapter

oc process -f ..\phsa-adapter\phsa-adapter.yml.template.yml --param-file ..\..\..\params\phsa-adapter.yml.test.params | oc apply -f -