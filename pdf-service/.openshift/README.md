# Openshift CLI

Use Template to perform the initial OpenShift setup and initial deployment.  Subsequent builds and deployments triggered by GitHub Actions.  Openshift token must be saved as a GitHub Action secret for this repo.

##
## 1. Common Setup

### Openshift: Create/update ImageStreams (source & destination)
```bash
oc project <image ns>
oc process -f image.yaml  |  oc apply -f -
oc describe is/node
oc describe is/formio-pdf
```
### Set policy to allow runtime namespace to pull from image namespace
```bash
oc policy add-role-to-user system:image-puller system:serviceaccount:<runtime ns>:default -n=<image ns>
```

### Create Deployment in the runtime namespace
```bash
oc project <runtime image namespace>
oc process -p IMAGE_NS=<image ns> -f formio-pdf.yaml  |  oc apply -f -
```

##
## 2. Build Image on GitHub & Push to OpenShift

### GitHub: Create Action Secrets
- `OPENSHIFT_REGISTRY`   : image-registry.apps.silver.devops.gov.bc.ca
- `OPENSHIFT_NAMESPACE`  : Namespace that contains the ImageStream
- `OPENSHIFT_TOKEN`      :tokrn for service account (see below)

### Run GitHub Action "OpenShift Connection Test"
Confirm Action runs without errors and logs into your OpenShift

### Run GitHub Action "Build on GitHub & Push to OpenShift"

## add redeploy trigger for when image changes
## Not needed with this template, already included
```bash
oc set triggers deploy/formio-pdf --from-image=formio-pdf:latest -c formio-pdf
```

## Generate / fetch OpenShift token for GitHub actions.
This Allows GitHub Action to push the image to the OpenShift image registry

### Check if Service account exists
 ```
 oc get sa github-action-sa
 ```

### Create service account (if not already there)
```
oc create serviceaccount -n <tools-ns> github-action-sa
oc policy add-role-to-user edit -z github-action-sa -n <tools-ns>
```

### Create a long-lived token for this sa
```
oc apply -f sa_token.yaml
```

### Fetch the long-lived token for the sa
```
oc get secret github-action-sa-token -n <tools-ns> -o jsonpath='{.data.token}' | base64 -d
```

###  Test token in https://jwt.io to confirm no expiry
