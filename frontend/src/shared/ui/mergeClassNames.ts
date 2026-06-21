import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

/**
 * Builds Tailwind class strings from conditional inputs and removes conflicting utilities.
 *
 * Useful for shared UI components that accept a `className` override. For example,
 * `mergeClassNames('px-4 bg-brand-primary', 'px-8')` returns a class string where
 * the caller's `px-8` cleanly overrides the default `px-4`.
 */
export const mergeClassNames = (...inputs: ClassValue[]) => twMerge(clsx(inputs));
