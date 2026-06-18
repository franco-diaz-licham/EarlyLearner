import '@testing-library/jest-dom/vitest';
import { render, screen } from '@testing-library/react';
import { HomePage } from './HomePage';

describe('HomePage', () => {
  test('renders the simple parent home', () => {
    render(<HomePage />);

    expect(screen.getByRole('heading', { name: 'Good morning, Franco!' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Recent Moments' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'School Readiness' })).toBeInTheDocument();
  });
});
