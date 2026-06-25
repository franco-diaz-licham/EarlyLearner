import type { ButtonProps } from 'primereact/button';
import { Button } from 'primereact/button';
import { mergeClassNames } from './mergeClassNames';

type ButtonVariant = 'primary' | 'secondary';

interface AppButtonProps extends ButtonProps {
  variant?: ButtonVariant;
}

export const AppButton = ({ children, className, disabled, variant = 'primary', ...buttonProps }: AppButtonProps) => {
  return (
    <Button
      {...buttonProps}
      disabled={disabled}
      className={mergeClassNames(
        'inline-flex min-h-10 items-center justify-center gap-2 rounded-md px-4',
        'text-sm font-semibold transition',
        'focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-primary',
        disabled && 'cursor-not-allowed opacity-60',
        variant === 'primary' && 'bg-brand-primary text-white',
        variant === 'primary' && !disabled && 'hover:bg-brand-primary-hover',
        variant === 'secondary' && 'border border-brand-border bg-brand-surface text-brand-heading',
        variant === 'secondary' && !disabled && 'hover:bg-brand-surface-muted',
        className
      )}
      type="button"
    >
      {children}
    </Button>
  );
};
