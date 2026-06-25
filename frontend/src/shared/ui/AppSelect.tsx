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

const selectClassName = mergeClassNames(
  'flex min-h-10 w-full items-center rounded-md border border-brand-border bg-brand-surface text-brand-text shadow-none transition',
  'focus-within:border-brand-primary focus-within:outline-none focus-within:ring-2 focus-within:ring-brand-primary/20'
);

const selectPassThrough = {
  input: {
    className: 'flex min-h-10 flex-1 items-center px-4 py-2 text-brand-text'
  },
  trigger: {
    className: 'flex w-10 items-center justify-center text-brand-heading'
  },
  panel: {
    className: 'mt-1 overflow-hidden rounded-md border border-brand-border bg-brand-surface shadow-app-card'
  },
  list: {
    className: 'p-1.5'
  },
  item: {
    className:
      'm-0 cursor-pointer rounded px-3 py-2.5 text-[0.95rem] leading-5 text-brand-text transition hover:bg-brand-surface-soft hover:text-brand-heading aria-selected:bg-brand-primary-soft aria-selected:font-bold aria-selected:text-brand-primary'
  },
  itemLabel: {
    className: 'block'
  }
};

export const AppSelect = <TValue extends string = string>({ className, id, label, options, required, value, onChange, ...selectProps }: AppSelectProps<TValue>) => {
  const select = (
    <Dropdown
      {...selectProps}
      aria-required={required}
      inputId={id}
      unstyled
      className={mergeClassNames(selectClassName, className)}
      optionLabel="label"
      optionValue="value"
      options={options}
      pt={selectPassThrough}
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
