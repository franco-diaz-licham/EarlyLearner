import { render, screen } from '@testing-library/react';
import { AppHero } from './AppHero';

describe('AppHero', () => {
  test('renders the home hero with the current user and child names', () => {
    // Act
    render(<AppHero childName="Mia" currentUserName="Franco" />);

    // Assert
    expect(screen.getByRole('heading', { name: 'Good morning, Franco!' })).toBeInTheDocument();
    expect(screen.getByText('Mia is curious, creative and growing every day.')).toBeInTheDocument();
  });

  test('renders fallback messages without names', () => {
    // Act
    render(<AppHero />);

    // Assert
    expect(screen.getByRole('heading', { name: 'Good morning!' })).toBeInTheDocument();
    expect(screen.getByText('Your learners are curious, creative and growing every day.')).toBeInTheDocument();
  });
});
