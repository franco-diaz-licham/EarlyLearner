import type { InputTextareaProps } from 'primereact/inputtextarea';
import { InputTextarea } from 'primereact/inputtextarea';

export const AppInputTextArea = ({ className, ...inputProps }: InputTextareaProps) => {
  let inputClassName = 'min-h-32 w-full rounded-md border border-brand-border px-4 py-3 text-brand-text transition placeholder:text-brand-muted focus:border-brand-primary focus:outline-none focus:ring-2 focus:ring-brand-primary/20';

  if (className) inputClassName = `${inputClassName} ${className}`;

  return <InputTextarea {...inputProps} className={inputClassName} />;
};
