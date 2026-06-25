import { Dropdown } from 'primereact/dropdown';
import type { DropdownProps } from 'primereact/dropdown';
import type { ReactNode } from 'react';
import { mergeClassNames } from './mergeClassNames';

export interface AppSelectOption<TValue extends string = string> {
  label: string;
  value: TValue;
}

interface AppSelectProps<TValue extends string = string> extends Omit<DropdownProps, 'onChange' | 'optionLabel' | 'optionValue' | 'options' | 'value'> {
  label?: ReactNode;
  options: AppSelectOption<TValue>[];
  value: TValue;
  onChange: (value: TValue) => void;
}

export const AppSelect = <TValue extends string = string>({ className, id, label, options, required, value, onChange, ...selectProps }: AppSelectProps<TValue>) => {
  const select = (
    <Dropdown
      {...selectProps}
      inputId={id}
      required={required}
      className={mergeClassNames('min-h-10 w-full border-brand-border text-brand-text', className)}
      optionLabel="label"
      optionValue="value"
      options={options}
      value={value}
      onChange={(event) => {
        onChange(event.value as TValue);
      }}
    />
  );

  if (!label) return select;

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
      {select}
    </label>
  );
};
