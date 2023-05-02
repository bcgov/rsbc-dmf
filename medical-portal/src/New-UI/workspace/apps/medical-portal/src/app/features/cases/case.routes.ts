export class CaseRoutes {
  public static MODULE_PATH = 'cases';

  /**
   * @description
   * Useful for redirecting to module root-level routes.
   */
  public static routePath(route: string): string {
    return `/${CaseRoutes.MODULE_PATH}/${route}`;
  }
}
