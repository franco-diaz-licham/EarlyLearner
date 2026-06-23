import { useEffect, useRef } from 'react';
import { Toast } from 'primereact/toast';
import type { ToastMessage } from 'primereact/toast';
import { subscribeToFeedback } from './feedbackEvents';

export const AppErrorToast = () => {
  const toastRef = useRef<Toast>(null);

  useEffect(() => {
    return subscribeToFeedback((message) => {
      const toastMessage: ToastMessage = {
        ...message,
        life: message.severity === 'error' ? 7000 : 4000
      };

      toastRef.current?.show(toastMessage);
    });
  }, []);

  return <Toast ref={toastRef} position="top-right" />;
};
