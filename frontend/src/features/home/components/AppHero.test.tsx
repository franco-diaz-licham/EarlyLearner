import { render, screen } from '@testing-library/react';
import { AppHero } from './AppHero';

describe('AppHero', () => {
  test('renders the home hero', () => {
    // Act
    render(<AppHero />);

    // Assert
    expect(screen.getByRole('heading', { name: 'Good morning, Franco!' })).toBeInTheDocument();
    expect(screen.getByText('Sophia is curious, creative and growing every day.')).toBeInTheDocument();
  });
});
