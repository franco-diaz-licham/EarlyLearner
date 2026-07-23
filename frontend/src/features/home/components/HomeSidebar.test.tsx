import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import type { HomeModel } from '../types/home.types';
import { HomeSidebar } from './HomeSidebar';

const home: HomeModel = {
  children: [
    {
      childId: 'child-1',
      givenName: 'Mia',
      dateOfBirth: '2021-04-15',
      avatarStoredFileId: null
    }
  ],
  metrics: [
    {
      label: 'Open invites',
      value: 2,
      detail: 'Invitations waiting for a response.'
    }
  ],
  recentActivities: [],
  today: {
    dailyLogCount: 1,
    learningMomentCount: 3,
    childrenObservedCount: 1
  },
  outcomeCoverage: {
    activeOutcomeCount: 12,
    touchedThisWeekCount: 5,
    untouchedActiveOutcomeCount: 7
  },
  recentMoments: []
};

const renderSidebar = (overrides: Partial<Parameters<typeof HomeSidebar>[0]> = {}) =>
  render(
    <MemoryRouter>
      <HomeSidebar home={home} isLoading={false} {...overrides} />
    </MemoryRouter>
  );

describe('HomeSidebar', () => {
  test('renders learning coverage and household pulse', () => {
    // Act
    renderSidebar();

    // Assert
    expect(screen.getByRole('heading', { name: 'Learning Coverage' })).toBeInTheDocument();
    expect(screen.getByText('Active')).toBeInTheDocument();
    expect(screen.getByText('Used')).toBeInTheDocument();
    expect(screen.getByText('Not used')).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Manage outcomes' })).toHaveAttribute('href', '/outcomes');
    expect(screen.getByRole('heading', { name: 'Household Pulse' })).toBeInTheDocument();
    expect(screen.getByText('Open invites')).toBeInTheDocument();
    expect(screen.getByText('Invitations waiting for a response.')).toBeInTheDocument();
  });

  test('renders loading placeholders', () => {
    // Act
    renderSidebar({ home: undefined, isLoading: true });

    // Assert
    expect(screen.getAllByText('-')).toHaveLength(3);
  });

  test('renders without metrics', () => {
    // Act
    renderSidebar({ home: { ...home, metrics: [] } });

    // Assert
    expect(screen.getByRole('heading', { name: 'Household Pulse' })).toBeInTheDocument();
    expect(screen.queryByText('Open invites')).not.toBeInTheDocument();
  });
});


