# SharedPortalUi

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 16.3.4.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.

 ### Shared Library

 
 - To use the shared libraries in the main application follow the steps:
 1. In angular.json file add "preserveSymlinks" : true
  - "projects.project-name.architect.build.options.preserveSymlinks": true
 2. In the tsconfig.json file
 paths: {
   "@angular/*": [
      "./node_modules/@angular/*"
   ],
}

3. Reference links for
https://github.com/angular/angular/issues/35586#issuecomment-630774572
https://stackoverflow.com/questions/64060616/angular-10-cannot-read-property-bindingstartindex-of-null-when-using-library-w

3. Add the necessary shared components in this project and reference the module in the main app.module.ts file
4. Note: You may have to install node modules in the root of shared library to get the editor support.
5. Export all the modules, components, services , pipes etc in the public-api.ts file



### How to use shared libraries

Step 1 : Move the component that need to be shared into shared-portal-ui/projevts/core-ui/src/lib
Step 2 : Export the component in public-api.ts 
Step 3: Build the shared-portal-ui 
         npm run watch
Step 4 : To use the shared component in the portals need to delete the ANGULAR folder and start the application
         (NOTE: deleting the angular folder because of the cached files)
         
