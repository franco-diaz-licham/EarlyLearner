import type { SelectHTMLAttributes } from 'react';
import { mergeClassNames } from './mergeClassNames';

export interface AppSelectOption<TValue extends string = string> {
  label: string;
  value: TValue;
}

interface AppSelectProps<TValue extends string = string> extends Omit<SelectHTMLAttributes<HTMLSelectElement>, 'onChange'> {
  options: AppSelectOption<TValue>[];
  onChange: (value: TValue) => void;
}

export const AppSelect = <TValue extends string = string>({ className, options, onChange, ...selectProps }: AppSelectProps<TValue>) => {
  return (
    <select
      {...selectProps}
      className={mergeClassNames('min-h-10 w-full rounded-md border border-brand-border bg-white px-4 py-2 text-brand-text transition focus:border-brand-primary focus:outline-none focus:ring-2 focus:ring-brand-primary/20', className)}
      onChange={(event) => {
        onChange(event.target.value as TValue);
      }}
    >
      {options.map((option) => (
        <option key={option.value} value={option.value}>
          {option.label}
        </option>
      ))}
    </select>
  );
};
