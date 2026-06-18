import type { ButtonProps } from 'primereact/button';
import { Button } from 'primereact/button';
type ButtonVariant = 'primary' | 'secondary';

interface AppButtonProps extends ButtonProps {
  variant?: ButtonVariant;
}

export const AppButton = ({ children, className, variant = 'primary', ...buttonProps }: AppButtonProps) => {
  let buttonClassName =
    'inline-flex min-h-10 items-center justify-center gap-2 rounded-md bg-brand-primary px-4 text-sm font-semibold text-white transition hover:bg-brand-primary-hover focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-primary';

  if (variant === 'secondary') {
    buttonClassName =
      'inline-flex min-h-10 items-center justify-center gap-2 rounded-md border border-brand-border bg-brand-surface px-4 text-sm font-semibold text-brand-heading transition hover:bg-brand-surface-muted focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-primary';
  }

  if (className) buttonClassName = `${buttonClassName} ${className}`;

  return (
    <Button {...buttonProps} className={buttonClassName} type="button">
      {children}
    </Button>
  );
};
