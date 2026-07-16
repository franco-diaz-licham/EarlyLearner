import { render, screen } from '@testing-library/react';
import { LearningTodaySummaryCard } from './LearningTodaySummaryCard';

describe('LearningTodaySummaryCard', () => {
  test('renders today learning counts', () => {
    // Act
    render(<LearningTodaySummaryCard activityCount={3} readingCount={2} routineCount={1} />);

    // Assert
    expect(screen.getByRole('heading', { name: 'Today at a Glance' })).toBeInTheDocument();
    expect(screen.getByText('Activities')).toBeInTheDocument();
    expect(screen.getByText('Reading')).toBeInTheDocument();
    expect(screen.getByText('Routine')).toBeInTheDocument();
    expect(screen.getByText('3')).toBeInTheDocument();
    expect(screen.getByText('2')).toBeInTheDocument();
    expect(screen.getByText('1')).toBeInTheDocument();
  });
});
