# Shared Library UI

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 16.3.4.

Run the following commands from `/projects/core-ui`

## BUILD

`ng b`

### To install in new projects

npm install "..\..\..\shared-portal-ui\dist\core-ui"

### Archived, may or may not be needed

- To use the shared libraries in the main application follow the steps:

1.  In angular.json file add "preserveSymlinks" : true

- "projects.project-name.architect.build.options.preserveSymlinks": true

2. In the tsconfig.json file
   paths: {
   "@angular/_": [
   "./node_modules/@angular/_"
   ],
   }

3. Reference links for
   https://github.com/angular/angular/issues/35586#issuecomment-630774572
   https://stackoverflow.com/questions/64060616/angular-10-cannot-read-property-bindingstartindex-of-null-when-using-library-w

4. Add the necessary shared components in this project and reference the module in the main app.module.ts file
5. Note: You may have to install node modules in the root of shared library to get the editor support.
6. Export all the modules, components, services , pipes etc in the public-api.ts file

### How to use shared libraries

Step 1 : Move the component that need to be shared into shared-portal-ui/projevts/core-ui/src/lib
Step 2 : Export the component in public-api.ts 
Step 3: Build the shared-portal-ui 
         npm run watch
Step 4 : To use the shared component in the portals need to delete the ANGULAR folder and start the application
         (NOTE: deleting the angular folder because of the cached files)