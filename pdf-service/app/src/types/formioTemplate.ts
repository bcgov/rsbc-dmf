export interface FormioTemplate {
  display: "form" | string;
  type: "form";
  components: any[];
  [key: string]: any; // just in case there is more keys
}
