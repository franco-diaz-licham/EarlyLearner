import type { InputTextProps } from 'primereact/inputtext';
import { InputText } from 'primereact/inputtext';
import type { ReactNode } from 'react';
import { useId } from 'react';
import { mergeClassNames } from './mergeClassNames';

interface AppInputTextProps extends InputTextProps {
  error?: ReactNode;
  label?: ReactNode;
}

export const AppInputText = ({ className, error, id, label, required, ...inputProps }: AppInputTextProps) => {
  const generatedId = useId();
  const inputId = id ?? generatedId;
  const errorId = `${inputId}-error`;
  const input = (
    <InputText
      {...inputProps}
      aria-describedby={error ? errorId : inputProps['aria-describedby']}
      aria-invalid={error ? true : inputProps['aria-invalid']}
      id={inputId}
      required={required}
      className={mergeClassNames(
        'min-h-10 w-full rounded-md border border-brand-border px-4 py-2 text-brand-text transition placeholder:text-brand-muted focus:border-brand-primary focus:outline-none focus:ring-2 focus:ring-brand-primary/20',
        className
      )}
    />
  );

  if (!label && !error) return input;

  return (
    <div className="block">
      {label ? (
        <label className="mb-2 block text-sm font-semibold text-brand-heading" htmlFor={inputId}>
          {label}
          {required ? (
            <>
              <span aria-hidden="true" className="ml-1 text-red-600">
                *
              </span>
              <span className="sr-only"> required</span>
            </>
          ) : null}
        </label>
      ) : null}
      {input}
      {error ? (
        <span className="mt-1 block text-sm font-semibold text-red-600" id={errorId}>
          {error}
        </span>
      ) : null}
    </div>
  );
};
