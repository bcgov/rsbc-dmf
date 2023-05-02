export class CaseAssistanceRoutes {
  public static MODULE_PATH = 'caseAssistance';

  /**
   * @description
   * Useful for redirecting to module root-level routes.
   */
  public static routePath(route: string): string {
    return `/${CaseAssistanceRoutes.MODULE_PATH}/${route}`;
  }
}
