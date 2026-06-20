import type { DialogProps, DialogPassThroughOptions } from 'primereact/dialog';
import { Dialog } from 'primereact/dialog';

const appDialogPt: DialogPassThroughOptions = {
  closeButton: {
    className:
      'flex h-9 w-9 items-center justify-center rounded-2xl border border-transparent bg-transparent text-brand-muted transition hover:border-brand-border hover:bg-brand-surface-muted hover:text-brand-heading focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-primary'
  },
  closeButtonIcon: {
    className: 'h-4 w-4'
  },
  content: {
    className: 'bg-white px-6 pb-6 pt-1 text-brand-text'
  },
  header: {
    className: 'flex items-center justify-between gap-4 border-b border-brand-border bg-white px-6 py-5'
  },
  headerTitle: {
    className: 'text-lg font-bold text-brand-heading'
  },
  mask: {
    className: 'bg-brand-heading/45 p-4 backdrop-blur-sm'
  },
  root: {
    className: 'max-h-[calc(100vh-2rem)]'
  }
};

export const AppDialog = ({ children, className, pt, ...dialogProps }: DialogProps) => {
  let dialogClassName = 'w-[min(92vw,560px)] overflow-hidden rounded-2xl! border! border-brand-border! bg-white! shadow-[0_24px_64px_rgba(23,25,35,0.18)]!';

  if (className) dialogClassName = `${dialogClassName} ${className}`;

  return (
    <Dialog {...dialogProps} blockScroll className={dialogClassName} dismissableMask modal pt={{ ...appDialogPt, ...pt }}>
      {children}
    </Dialog>
  );
};
