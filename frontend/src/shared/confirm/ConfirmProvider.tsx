import { useCallback, useState, type PropsWithChildren } from 'react';
import { ConfirmContext, type Confirm, type ConfirmOptions } from './confirmContext';
import { AppButton } from '../ui/AppButton';
import { AppDialog } from '../ui/AppDialog';

interface PendingConfirm extends Required<ConfirmOptions> {
  resolve: (confirmed: boolean) => void;
}

export const ConfirmProvider = ({ children }: PropsWithChildren) => {
  const [pendingConfirm, setPendingConfirm] = useState<PendingConfirm | null>(null);

  const confirm = useCallback<Confirm>((options) => {
    return new Promise<boolean>((resolve) => {
      setPendingConfirm({
        title: options.title ?? 'Confirm',
        message: options.message,
        confirmLabel: options.confirmLabel ?? 'Confirm',
        cancelLabel: options.cancelLabel ?? 'Cancel',
        resolve
      });
    });
  }, []);

  const resolveConfirm = (confirmed: boolean) => {
    pendingConfirm?.resolve(confirmed);
    setPendingConfirm(null);
  };

  return (
    <ConfirmContext.Provider value={confirm}>
      {children}
      <AppDialog
        header={pendingConfirm?.title}
        visible={pendingConfirm !== null}
        onHide={() => {
          resolveConfirm(false);
        }}
      >
        <div className="space-y-6">
          <p className="text-sm leading-6 text-brand-text">{pendingConfirm?.message}</p>
          <div className="flex justify-end gap-3">
            <AppButton
              variant="secondary"
              onClick={() => {
                resolveConfirm(false);
              }}
            >
              {pendingConfirm?.cancelLabel}
            </AppButton>
            <AppButton
              onClick={() => {
                resolveConfirm(true);
              }}
            >
              {pendingConfirm?.confirmLabel}
            </AppButton>
          </div>
        </div>
      </AppDialog>
    </ConfirmContext.Provider>
  );
};
