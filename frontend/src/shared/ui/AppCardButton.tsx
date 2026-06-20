import type { ButtonHTMLAttributes, ReactNode } from 'react';

interface AppCardButtonProps extends Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'title'> {
  action?: ReactNode;
  icon?: ReactNode;
  selected?: boolean;
  supportingText?: ReactNode;
  title: ReactNode;
}

export const AppCardButton = ({ action, className, icon, selected = false, supportingText, title, ...buttonProps }: AppCardButtonProps) => {
  let buttonClassName = `rounded-md border p-4 text-left transition hover:cursor-pointer ${selected ? 'border-brand-primary bg-brand-primary-soft' : 'border-brand-border bg-white hover:bg-brand-surface-muted'}`;

  if (className) buttonClassName = `${buttonClassName} ${className}`;

  return (
    <button {...buttonProps} className={buttonClassName} type="button">
      {(icon ?? action) && (
        <div className="flex items-center justify-between gap-3 ">
          {icon}
          {action}
        </div>
      )}
      <h2 className="mt-4 font-bold text-brand-heading">{title}</h2>
      {supportingText && <p className="mt-2 text-sm leading-6 text-brand-muted">{supportingText}</p>}
    </button>
  );
};
