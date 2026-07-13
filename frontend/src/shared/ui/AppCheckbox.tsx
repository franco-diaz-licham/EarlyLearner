import type { CheckboxProps } from 'primereact/checkbox';
import { Checkbox } from 'primereact/checkbox';
import type { ReactNode } from 'react';
import { useId } from 'react';
import { mergeClassNames } from './mergeClassNames';

interface AppCheckboxProps extends CheckboxProps {
  containerClassName?: string;
  error?: ReactNode;
  label?: ReactNode;
}

export const AppCheckbox = ({ className, containerClassName, error, inputId, label, required, ...checkboxProps }: AppCheckboxProps) => {
  const generatedId = useId();
  const checkboxId = inputId ?? generatedId;
  const errorId = `${checkboxId}-error`;
  const checkbox = (
    <Checkbox
      {...checkboxProps}
      aria-describedby={error ? errorId : checkboxProps['aria-describedby']}
      aria-invalid={error ? true : checkboxProps['aria-invalid']}
      className={mergeClassNames(
        'shrink-0 [&_.p-checkbox-box]:flex [&_.p-checkbox-box]:h-5 [&_.p-checkbox-box]:w-5 [&_.p-checkbox-box]:items-center [&_.p-checkbox-box]:justify-center [&_.p-checkbox-box]:rounded [&_.p-checkbox-box]:border [&_.p-checkbox-box]:border-brand-border [&_.p-checkbox-box]:bg-white [&_.p-checkbox-box]:text-white [&_.p-checkbox-box]:transition [&_.p-checkbox-box]:duration-150 [&_.p-checkbox-box.p-highlight]:border-brand-primary [&_.p-checkbox-box.p-highlight]:bg-brand-primary [&_.p-checkbox-icon]:h-3 [&_.p-checkbox-icon]:w-3 [&_.p-checkbox-input]:cursor-pointer',
        className
      )}
      inputId={checkboxId}
      required={required}
    />
  );

  if (!label && !error) return checkbox;

  return (
    <div>
      <label className={mergeClassNames('flex cursor-pointer items-start gap-3 rounded-md border border-brand-border bg-white p-3 text-sm text-brand-text transition hover:bg-brand-surface-soft', containerClassName)} htmlFor={checkboxId}>
        {checkbox}
        {label ? (
          <span>
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
        ) : null}
      </label>
      {error ? (
        <span className="mt-1 block text-sm font-semibold text-red-600" id={errorId}>
          {error}
        </span>
      ) : null}
    </div>
  );
};
