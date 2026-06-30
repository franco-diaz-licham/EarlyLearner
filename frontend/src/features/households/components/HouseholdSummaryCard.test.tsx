import { render, screen } from '@testing-library/react';
import { HouseholdSummaryCard } from './HouseholdSummaryCard';

describe('HouseholdSummaryCard', () => {
  test('renders the title and children', () => {
    // Act
    render(
      <HouseholdSummaryCard title="Household Members">
        <p>Member content</p>
      </HouseholdSummaryCard>
    );

    // Assert
    expect(screen.getByRole('heading', { name: 'Household Members' })).toBeInTheDocument();
    expect(screen.getByText('Member content')).toBeInTheDocument();
  });

  test('renders an empty message while keeping children mounted', () => {
    // Act
    render(
      <HouseholdSummaryCard emptyMessage="No members yet." isEmpty title="Household Members">
        <p>Empty state child content</p>
      </HouseholdSummaryCard>
    );

    // Assert
    expect(screen.getByText('No members yet.')).toBeInTheDocument();
    expect(screen.getByText('Empty state child content')).toBeInTheDocument();
  });

  test('does not render an empty message when the card is not empty', () => {
    // Act
    render(
      <HouseholdSummaryCard emptyMessage="No members yet." title="Household Members">
        <p>Member content</p>
      </HouseholdSummaryCard>
    );

    // Assert
    expect(screen.queryByText('No members yet.')).not.toBeInTheDocument();
  });
});
