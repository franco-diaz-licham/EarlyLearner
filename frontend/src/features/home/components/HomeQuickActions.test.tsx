import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { HomeQuickActions } from './HomeQuickActions';

const renderQuickActions = () =>
  render(
    <MemoryRouter>
      <HomeQuickActions />
    </MemoryRouter>
  );

describe('HomeQuickActions', () => {
  test('renders the home navigation actions', () => {
    // Act
    renderQuickActions();

    // Assert
    expect(screen.getByRole('link', { name: 'Manage household' })).toHaveAttribute('href', '/households');
    expect(screen.getByRole('link', { name: 'View logs' })).toHaveAttribute('href', '/learning');
    expect(screen.getByRole('link', { name: 'Manage outcomes' })).toHaveAttribute('href', '/outcomes');
  });
});
