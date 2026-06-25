import type { ButtonProps } from 'primereact/button';
import { Button } from 'primereact/button';
import { mergeClassNames } from './mergeClassNames';

interface AppIconButtonProps extends ButtonProps {
  'aria-label': string;
}

export const AppIconButton = ({ children, className, ...buttonProps }: AppIconButtonProps) => {
  return (
    <Button {...buttonProps} className={mergeClassNames('app-icon-button hidden h-10 w-10 items-center justify-center rounded-full border-0 bg-transparent p-0 text-brand-heading hover:bg-brand-surface-muted sm:flex', className)} type="button">
      {children}
    </Button>
  );
};
