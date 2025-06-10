import { IFormioCell, IFormioColumn, IFormioComponent, IFormioTemplate } from '../types/serviceInterfaces';

/**
 * Processes a Formio template and returns a modified version.
 *
 * @export
 * @param {IFormioTemplate} template 
 * @param {Set<string>} lookup 
 * @returns {IFormioTemplate} 
 */
export function processTemplate(
    template: IFormioTemplate,
    lookup: Set<string>
): IFormioTemplate {
  const updatedTemplate = { ...template };

  updatedTemplate.components = processComponents(template.components, lookup);

  return updatedTemplate;
}

/**
 * Process components that have data to display
 * Recursively traverses a Form.io component tree and updates any component
 * 
 * @export
 * @param {IFormioComponent[]} components 
 * @param {Set<string>} lookup 
 * @returns {IFormioComponent[]} 
 */
export function processComponents(components: IFormioComponent[], lookup: Set<string>): IFormioComponent[] {
  return components.map(component => {
    const updated: IFormioComponent = { ...component };

    // If the key is in the lookup set, uncollapse it
    if (typeof updated.key === 'string' && lookup.has(updated.key)) {
      updated.collapsed = false;
    }

    // Recurse into nested components
    if (Array.isArray(updated.components)) {
      updated.components = processComponents(updated.components, lookup);
    }

    // Handle columns layout
    if (Array.isArray(updated.columns)) {
      updated.columns = updated.columns.map((column: IFormioColumn) => ({
        ...column,
        components: processComponents(column.components || [], lookup)
      }));
    }

    // Handle table or row/column layout
    if (Array.isArray(updated.rows)) {
      updated.rows = updated.rows.map((row: IFormioCell[]) =>
        row.map((cell: IFormioCell) => ({
          ...cell,
          components: processComponents(cell.components || [], lookup)
        }))
      );
    }

    return updated;
  });
}
