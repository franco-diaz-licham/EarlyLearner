import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import type { HomeOutcomeCoverageModel } from '../types/home.types';
import { LearningCoverageCard } from './LearningCoverageCard';

const coverage: HomeOutcomeCoverageModel = {
  activeOutcomeCount: 12,
  touchedThisWeekCount: 5,
  untouchedActiveOutcomeCount: 7
};

const renderCoverageCard = (overrides: Partial<Parameters<typeof LearningCoverageCard>[0]> = {}) =>
  render(
    <MemoryRouter>
      <LearningCoverageCard coverage={coverage} isLoading={false} {...overrides} />
    </MemoryRouter>
  );

describe('LearningCoverageCard', () => {
  test('renders outcome coverage totals', () => {
    // Act
    renderCoverageCard();

    // Assert
    expect(screen.getByRole('heading', { name: 'Learning Coverage' })).toBeInTheDocument();
    expect(screen.getByText('Outcome use over the last seven days.')).toBeInTheDocument();
    expect(screen.getByText('12')).toBeInTheDocument();
    expect(screen.getByText('5')).toBeInTheDocument();
    expect(screen.getByText('7')).toBeInTheDocument();
    expect(screen.getByText('Active')).toBeInTheDocument();
    expect(screen.getByText('Used')).toBeInTheDocument();
    expect(screen.getByText('Not used')).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Manage outcomes' })).toHaveAttribute('href', '/outcomes');
  });

  test('renders loading placeholders', () => {
    // Act
    renderCoverageCard({ coverage: undefined, isLoading: true });

    // Assert
    expect(screen.getAllByText('-')).toHaveLength(3);
  });
});
