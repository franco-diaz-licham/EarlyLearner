import type { InputTextProps } from 'primereact/inputtext';
import { InputText } from 'primereact/inputtext';
import type { ReactNode } from 'react';
import { mergeClassNames } from './mergeClassNames';

interface AppInputTextProps extends InputTextProps {
  label?: ReactNode;
}

export const AppInputText = ({ className, id, label, required, ...inputProps }: AppInputTextProps) => {
  const input = (
    <InputText
      {...inputProps}
      id={id}
      required={required}
      className={mergeClassNames(
        'min-h-10 w-full rounded-md border border-brand-border px-4 py-2 text-brand-text transition placeholder:text-brand-muted focus:border-brand-primary focus:outline-none focus:ring-2 focus:ring-brand-primary/20',
        className
      )}
    />
  );

  if (!label) return input;

  return (
    <label className="block" htmlFor={id}>
      <span className="mb-2 block text-sm font-semibold text-brand-heading">
        {label}
        {required ? (
          <>
            <span aria-hidden="true" className="ml-1 text-red-600">
              *
            </span>
            <span className="sr-only"> required</span>
          </>
        ) : null}
      </span>
      {input}
    </label>
  );
};
