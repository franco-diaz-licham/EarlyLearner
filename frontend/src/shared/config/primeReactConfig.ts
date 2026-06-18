import type { ComponentProps } from 'react';
import { PrimeReactProvider } from 'primereact/api';

type PrimeReactProviderValue = NonNullable<ComponentProps<typeof PrimeReactProvider>['value']>;

export const primeReactConfig: PrimeReactProviderValue = {
  ripple: true
};
