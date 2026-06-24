import { Dropdown } from 'primereact/dropdown';
import type { DropdownProps } from 'primereact/dropdown';
import { mergeClassNames } from './mergeClassNames';

export interface AppSelectOption<TValue extends string = string> {
  label: string;
  value: TValue;
}

interface AppSelectProps<TValue extends string = string> extends Omit<DropdownProps, 'onChange' | 'optionLabel' | 'optionValue' | 'options' | 'value'> {
  options: AppSelectOption<TValue>[];
  value: TValue;
  onChange: (value: TValue) => void;
}

export const AppSelect = <TValue extends string = string>({ className, options, value, onChange, ...selectProps }: AppSelectProps<TValue>) => {
  return (
    <Dropdown
      {...selectProps}
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
};
