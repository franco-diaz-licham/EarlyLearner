import { useEffect, useRef } from 'react';
import { Toast } from 'primereact/toast';
import type { ToastMessage } from 'primereact/toast';
import { subscribeToFeedback, type AppFeedbackSeverity } from '../feedback/feedbackEvents';

const toastMessageOptionsBySeverity: Record<AppFeedbackSeverity, Pick<ToastMessage, 'severity' | 'life'>> = {
  error: { severity: 'error', life: 7000 },
  info: { severity: 'info', life: 4000 },
  success: { severity: 'success', life: 4000 },
  warn: { severity: 'warn', life: 4000 }
};

export const AppToast = () => {
  const toastRef = useRef<Toast>(null);

  useEffect(() => {
    return subscribeToFeedback((message) => {
      const toastMessage: ToastMessage = {
        ...message,
        ...toastMessageOptionsBySeverity[message.severity]
      };

      toastRef.current?.show(toastMessage);
    });
  }, []);

  return <Toast ref={toastRef} position="top-right" />;
};
