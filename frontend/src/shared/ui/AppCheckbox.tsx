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
  const isChecked = Boolean(checkboxProps.checked);
  const checkbox = (
    <Checkbox
      {...checkboxProps}
      aria-describedby={error ? errorId : checkboxProps['aria-describedby']}
      aria-invalid={error ? true : checkboxProps['aria-invalid']}
      className={className}
      inputId={checkboxId}
      pt={{ input: { className: 'sr-only' } }}
      required={required}
    />
  );

  if (!label && !error) return checkbox;

  return (
    <div>
      <label className={mergeClassNames('flex cursor-pointer items-start gap-3 rounded-md border border-brand-border p-3 text-sm text-brand-text transition hover:bg-brand-surface-soft', isChecked ? 'bg-brand-primary-soft' : 'bg-white', containerClassName)} htmlFor={checkboxId}>
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
