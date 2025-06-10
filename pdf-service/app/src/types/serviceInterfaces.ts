/**
 * The root structure of a Form.io template (form definition).
 * Represents the top-level object used when defining a Form.io form.
 *
 * @export
 * @interface IFormioTemplate
 * @typedef {IFormioTemplate}
 */
export interface IFormioTemplate {
  /**
   * Display mode of the form.
   * Common values include "form", "wizard", or "pdf".
   */
  display: "form" | string;

  /**
   * Type of template. Typically "form" for standard Form.io definitions.
   */
  type: "form";

  /**
   * Top-level components within the form.
   */
  components: IFormioComponent[];

  /**
   * Catch-all for any additional Form.io template-level properties.
   * Useful for extension, metadata, and custom behaviors.
   */
  [key: string]: any; // just in case there is more keys
}

/**
 * Represents a generic Form.io component.
 * Components are the building blocks of forms (e.g., inputs, panels, columns).
 *
 * @export
 * @interface IFormioComponent
 * @typedef {IFormioComponent}
 */
export interface IFormioComponent {
  /**
   * Unique key for the component, used for form data mapping.
   */
  key?: string;

  /**
   * Indicates whether the component (typically a panel) is collapsed by default.
   */
  collapsed?: boolean;

  /**
   * Nested components within this component (e.g., panel or fieldset).
   */
  components?: IFormioComponent[];

  /**
   * Column layout structure for multi-column layouts.
   */
  columns?: IFormioColumn[];

  /**
   * Grid/table-style row layout structure.
   */
  rows?: IFormioCell[][];

  /**
   * Catch-all for any additional Form.io component-level properties.
   * Allows extension with any other supported or custom fields
   */
  [key: string]: any;
}

/**
 * Represents a column within a Form.io column layout.
 * Columns allow arranging components horizontally.
 *
 * @export
 * @interface IFormioColumn
 * @typedef {IFormioColumn}
 */
export interface IFormioColumn {
  /**
   * Components contained in this column.
   */
  components: IFormioComponent[];
}

/**
 * Represents a cell within a Form.io table or grid layout
 * Each cell contains its own set of components.
 *
 * @export
 * @interface IFormioCell
 * @typedef {IFormioCell}
 */
export interface IFormioCell {
  /**
   * Components contained in this cell.
   */
  components: IFormioComponent[];
}

/**
 * Represents a standard JSON data wrapper for form submission values.
 *
 * @export
 * @interface IJsonData
 * @typedef {IJsonData}
 */
export interface IJsonData {
  /**
   * The submitted form data object, keyed by component `key` values.
   */
  data: {
    [key: string]: any;
  };
}
