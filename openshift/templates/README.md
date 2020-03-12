# OpenShift

OpenShift is the ministries on-premise cloud infrastructure for Kubernetes container orchestration and management application.

Refer to the following links for more information;

- Corporate Website - https://www.openshift.com/
- Corporate Website - https://kubernetes.io/
- Getting Started - https://www.openshift.com/learn/get-started/
- DevHub - https://developer.gov.bc.ca/
- DevHub - [Getting Started](https://developer.gov.bc.ca/Getting-Started-on-the-DevOps-Platform/12-Factor-Apps?intention=LOGIN#error=login_required)
- Community Docs - https://docs.pathfinder.gov.bc.ca/
- BCDevOps - https://github.com/bcdevops/devops-platform-workshops

## OpenShift Web Console

The Web Console provides a UI to manage, deploy, configure and maintain containers.

URL - https://console.pathfinder.gov.bc.ca/

## OpenShift CLI

While many things can be performed within the Web Console, most work requires the use of the Command Line Interface (CLI).

For Windows users I'd recommend using [Chocolately](https://chocolatey.org/) to install it (see link below).

- Getting Started - https://docs.openshift.com/container-platform/3.11/cli_reference/get_started_cli.html
- Command-Line Interface - https://github.com/openshift/origin/blob/master/docs/cli.md
- Download (scroll to relevant section) - https://github.com/openshift/origin/releases
- Install (with chocolately) - https://chocolatey.org/packages/openshift-cli
- Install - https://github.com/CCI-MOC/moc-public/wiki/Installing-the-oc-CLI-tool-of-OpenShift

## BCGov Source Control

The BC Government maintains a number of open-source project on GitHub. Many provide example projects that are built and running in OpenShift.

URL - https://github.com/bcgov
URL - https://github.com/bcdevops

Helpful Links;

- [OpenShift Wiki](https://github.com/BCDevOps/openshift-wiki)
- [OpenShift Developer Tools](https://github.com/BCDevOps/openshift-developer-tools/tree/master/bin)
- [Workshops](https://github.com/bcdevops/devops-platform-workshops)

## Projects

OpenShift provides a concept called **Projects** which are similar to Kubernetes **Namespaces**. These provide a way to grant users access to OpenShift and begin configuring and deploying containers.

The BC Government Policy is to create four **Projects** within OpenShift to support a real life `Project`, these are; TOOLS, DEV, TEST, PROD. Each represents a unique environment of which to configure, deploy and maintain the various components related to the `Project`.

| Project | Description                                                        |
| ------- | ------------------------------------------------------------------ |
| TOOLS   | Host your pipeline tools here (i.e. Jenkins)                       |
| DEV     | Development environment to build your solution and test scenarios. |
| TEST    | QA and or UAT to verify your solution is working as intended.      |
| PROD    | Production environment to host your live solution.                 |

## Templates

OpenShift enforces the concept of **Infrastructure as Code**. This implies that all configuration, environment, deployment, pipeline information is part of source-control.  
As such it is all maintained in the same location as the solution source-control, thus tightly coupled (although highly configurable).
This allows for setting up new environments consistently and with no lost knowledge.

**Templates** within OpenShift are YAML or JSON files that provide a way to setup, deploy and configure containers.
Once you create a template you can `import` the template into your OpenShift Project or simply `process` the template and it will setup the appropriate services.

The way this repository is organized is to provide all infrastructure related to OpenShift here `/openshift`.

- The **build** templates are located here `/openshift/templates/api/build.yaml`.
- The **deploy** templates are located here `/openshift/templates/api/deploy.yaml`.
- The **pipeline** templates are located here `/openshift/pipeline/api-pipeline.yaml`.
- Additionally **Jenkins** configuration pipeline line are located here `/openshift/jenkins/api/Jenkinsfile`.

Each of these provides the basic infrastructure to build and deploy the solution to OpenShift.

Essentially the structure provides the following workflow process; **TOOLS** will now have a **Jenkins** container with a **pipeline** that listens for _push_ webhook events from the GitHub repository. When it receives the event it will trigger the **build**. If the build is successful it will trigger the **deploy**. Currently this **pipeline** only _deploys_ to the **DEV** environment.
