/* tslint:disable */
/* eslint-disable */
export interface ApplicationVersionInfo {

  /**
   * Base Path of the application
   */
  basePath?: null | string;

  /**
   * Base URI for the application
   */
  baseUri?: null | string;

  /**
   * Dotnet Environment (Development, Staging, Production...)
   */
  environment?: null | string;

  /**
   * File creation time for the running assembly
   */
  fileCreationTime?: null | string;

  /**
   * File version for the running assembly
   */
  fileVersion?: null | string;

  /**
   * Git commit used to build the application
   */
  sourceCommit?: null | string;

  /**
   * Git reference used to build the application
   */
  sourceReference?: null | string;

  /**
   * Git repository used to build the application
   */
  sourceRepository?: null | string;
}
