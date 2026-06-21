import type { InputTextProps } from 'primereact/inputtext';
import { InputText } from 'primereact/inputtext';
import { mergeClassNames } from './mergeClassNames';

export const AppInputText = ({ className, ...inputProps }: InputTextProps) => {
  return (
    <InputText
      {...inputProps}
      className={mergeClassNames(
        'min-h-10 w-full rounded-md border border-brand-border px-4 py-2 text-brand-text transition placeholder:text-brand-muted focus:border-brand-primary focus:outline-none focus:ring-2 focus:ring-brand-primary/20',
        className
      )}
    />
  );
};
