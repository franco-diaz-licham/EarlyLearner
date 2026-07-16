import type { ReactNode } from 'react';
import { AppCheckbox } from './AppCheckbox';
import { mergeClassNames } from './mergeClassNames';

interface AppMultiCheckboxSelectorOption {
  inputId?: string;
  label: ReactNode;
  value: string;
}

interface AppMultiCheckboxSelectorProps {
  className?: string;
  error?: ReactNode;
  gridClassName?: string;
  label: ReactNode;
  options: AppMultiCheckboxSelectorOption[];
  required?: boolean;
  selectedValues: string[];
  onToggle: (value: string) => void;
}

export const AppMultiCheckboxSelector = ({ className, error, gridClassName, label, options, required, selectedValues, onToggle }: AppMultiCheckboxSelectorProps) => (
  <div className={className}>
    <p className="mb-2 text-sm font-semibold text-brand-heading">
      {label}
      {required ? (
        <span aria-hidden="true" className="ml-1 text-red-600">
          *
        </span>
      ) : null}
    </p>
    <div className={mergeClassNames('grid max-h-48 gap-2 overflow-y-auto pr-1 md:grid-cols-2', gridClassName)}>
      {options.map((option) => (
        <AppCheckbox
          checked={selectedValues.includes(option.value)}
          className="mt-1"
          inputId={option.inputId}
          key={option.value}
          label={option.label}
          onChange={() => {
            onToggle(option.value);
          }}
        />
      ))}
    </div>
    {error ? <span className="mt-1 block text-sm font-semibold text-red-600">{error}</span> : null}
  </div>
);
