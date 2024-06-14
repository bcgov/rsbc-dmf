export interface ChefsSubmission {
  status: string;
  submission: {
    [key: string]: any; // Use `any` for values of any type
  };
}
