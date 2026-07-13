import '@testing-library/jest-dom/vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { HomePage } from './HomePage';

const renderPage = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false
      }
    }
  });

  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>
    </QueryClientProvider>
  );
};

describe('HomePage', () => {
  test('renders the learning-aware home dashboard', () => {
    renderPage();

    expect(screen.getByRole('heading', { name: 'Good morning, Franco!' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Today' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Recent Learning' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Learning Coverage' })).toBeInTheDocument();
  });
});
