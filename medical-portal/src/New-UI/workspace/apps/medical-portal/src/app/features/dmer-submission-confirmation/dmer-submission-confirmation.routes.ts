export class DmerSubmissionRoutes {
  public static MODULE_PATH = 'dmerSubmissionConfirmation';

  /**
   * @description
   * Useful for redirecting to module root-level routes.
   */
  public static routePath(route: string): string {
    return `/${DmerSubmissionRoutes.MODULE_PATH}/${route}`;
  }
}
