import { render, screen } from '@testing-library/react';
import type { HomeMetricModel } from '../types/home.types';
import { HouseholdPulseCard } from './HouseholdPulseCard';

const metrics: HomeMetricModel[] = [
  {
    detail: 'Invitations waiting for a response.',
    label: 'Open invites',
    value: 2
  },
  {
    detail: 'Files uploaded today.',
    label: 'New files',
    value: 4
  }
];

describe('HouseholdPulseCard', () => {
  test('renders household metrics', () => {
    // Act
    render(<HouseholdPulseCard metrics={metrics} />);

    // Assert
    expect(screen.getByRole('heading', { name: 'Household Pulse' })).toBeInTheDocument();
    expect(screen.getByText('Small operational numbers for today.')).toBeInTheDocument();
    expect(screen.getByText('Open invites')).toBeInTheDocument();
    expect(screen.getByText('Invitations waiting for a response.')).toBeInTheDocument();
    expect(screen.getByText('New files')).toBeInTheDocument();
    expect(screen.getByText('Files uploaded today.')).toBeInTheDocument();
    expect(screen.getByText('2')).toBeInTheDocument();
    expect(screen.getByText('4')).toBeInTheDocument();
  });

  test('renders no metric rows when metrics are empty', () => {
    // Act
    render(<HouseholdPulseCard metrics={[]} />);

    // Assert
    expect(screen.getByRole('heading', { name: 'Household Pulse' })).toBeInTheDocument();
    expect(screen.queryByText('Open invites')).not.toBeInTheDocument();
  });
});
