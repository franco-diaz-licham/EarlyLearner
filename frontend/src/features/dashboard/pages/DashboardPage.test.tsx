import '@testing-library/jest-dom/vitest';
import { render, screen } from '@testing-library/react';
import { DashboardPage } from './DashboardPage';

describe('DashboardPage', () => {
  test('renders the simple parent dashboard', () => {
    render(<DashboardPage />);

    expect(screen.getByRole('heading', { name: 'Good morning, Franco!' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /log a moment/i })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'School Readiness' })).toBeInTheDocument();
  });
});
