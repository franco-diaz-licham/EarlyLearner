import { createContext, useContext } from 'react';

export interface ConfirmOptions {
  title?: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
}

export type Confirm = (options: ConfirmOptions) => Promise<boolean>;

export const ConfirmContext = createContext<Confirm | null>(null);

export const useConfirm = (): Confirm => {
  const confirm = useContext(ConfirmContext);
  if (confirm === null) throw new Error('useConfirm must be used within ConfirmProvider.');
  return confirm;
};
