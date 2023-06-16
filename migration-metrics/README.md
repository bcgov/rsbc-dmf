# Openshift setup and configuration

## Templates

### migration-metrics.template.yml

Template for Migration Metrics Deployment. Contains 2 deployment configs and attached resources.

API

- storage
  - data protection keys
- deployment config
- route, service and network policy
- config map for environment variables
- secret for environment variables

UI

- deployment config
- route, service and network policy

To create an environment:

1. create a new file named `migration-metrics.yml.<app name>.params` in the templates directory
1. copy the content from `migration-metrics.yml.params.template` into the file and fill in the values, these are the parameters supplied later to the template
1. login to openshift cli `oc login ... --token=...`
1. run the following command from cmd/powershell console (modify the Openshift project to the one you want to deploy to):

```cmd
oc process -f .\migration-metrics.template.yml --param-file .\migration-metrics.yml.<app name>.params | oc apply -f -
```

4. to update an existing environment, modify the templates and params, then execute the same command.

**Note: executing `oc apply` WILL trigger deployment, to test the changes add `--dry-run` at the end of the command**



