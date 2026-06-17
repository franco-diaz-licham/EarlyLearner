import type { PropsWithChildren } from "react";
import { QueryClientProvider } from "@tanstack/react-query";
import { PrimeReactProvider } from "primereact/api";
import { queryClient } from "../../shared/api/queryClient";

export const AppProviders = ({ children }: PropsWithChildren) => (
    <PrimeReactProvider value={{ ripple: true }}>
        <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    </PrimeReactProvider>
);
