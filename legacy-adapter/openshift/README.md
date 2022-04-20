# Openshift setup and configuration

## Templates

### icbc-adapter.template.yml

Template for ICBC adapter. Contains deployment config and attached resources.

- deployment config
- route, service and network policy

To create an environment:

1. create a new file named `icbc-adapter.yml.<app name>.params` in the templates directory
1. copy the content from icbc-adapter.yml.params.template` into the file and fill in the values, these are the parameters supplied later to the template
1. login to openshift cli `oc login ... --token=...`
1. run the following command from cmd/powershell console (modify the Openshift project to the one you want to deploy to):

```cmd
oc process -f .\icbc.template.yml --param-file .\icbc-adapter.yml.<app name>.params | oc apply -f -
```

4. to update an existing environment, modify the templates and params, then execute the same command.

**Note: executing `oc apply` WILL trigger deployment, to test the changes add `--dry-run` at the end of the command**




