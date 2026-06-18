import type { ButtonProps } from 'primereact/button';
import { Button } from 'primereact/button';

interface AppIconButtonProps extends ButtonProps {
  'aria-label': string;
}

export const AppIconButton = ({ children, className, ...buttonProps }: AppIconButtonProps) => {
  let buttonClassName = 'hidden h-10 w-10 items-center justify-center rounded-full border-0 bg-transparent p-0 text-brand-heading hover:bg-brand-surface-muted sm:flex';

  if (className) buttonClassName = `${buttonClassName} ${className}`;

  return (
    <Button {...buttonProps} className={buttonClassName} type="button">
      {children}
    </Button>
  );
};
