export class EndorsementRoutes {
  public static MODULE_PATH = 'endorsements';

  /**
   * @description
   * Useful for redirecting to module root-level routes.
   */
  public static routePath(route: string): string {
    return `/${EndorsementRoutes.MODULE_PATH}/${route}`;
  }
}
