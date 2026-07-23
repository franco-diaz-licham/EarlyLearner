import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { useSessionStore } from '../../../shared/stores/sessionStore';
import type { HomeModel } from '../types/home.types';
import { HomeMain } from './HomeMain';

const home: HomeModel = {
  children: [],
  metrics: [],
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
  recentMoments: [
    {
      dailyLogId: 'daily-log-1',
      learningMomentId: 'moment-1',
      childId: 'child-1',
      childName: 'Mia',
      logDate: '2026-07-16',
      kind: 'reading',
      title: 'Read a story',
      notes: 'Read a picture book and named familiar animals.',
      outcomeNames: ['Listens and responds']
    }
  ]
};

const renderMain = (overrides: Partial<Parameters<typeof HomeMain>[0]> = {}) =>
  render(
    <MemoryRouter>
      <HomeMain home={home} isLoading={false} {...overrides} />
    </MemoryRouter>
  );

describe('HomeMain', () => {
  beforeEach(() => {
    useSessionStore.getState().setCurrentUser({
      displayName: 'Franco',
      organisationName: 'Current household',
      roleLabel: 'Active'
    });
  });
  test('renders the learning overview', () => {
    // Act
    renderMain();

    // Assert
    expect(screen.getByRole('heading', { name: 'Good morning, Franco!' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Learning Overview' })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Manage household' })).toHaveAttribute('href', '/households');
    expect(screen.getByRole('link', { name: 'View logs' })).toHaveAttribute('href', '/learning');
    expect(screen.getByRole('link', { name: 'Manage outcomes' })).toHaveAttribute('href', '/outcomes');
    expect(screen.getByRole('heading', { name: 'Learning Mix' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Outcome Coverage' })).toBeInTheDocument();
    expect(screen.getByText('Reading')).toBeInTheDocument();
    expect(screen.getByText('Used this week')).toBeInTheDocument();
    expect(screen.queryByRole('heading', { name: 'Read a story' })).not.toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'View all' })).toHaveAttribute('href', '/learning');
  });

  test('renders an empty learning overview state', () => {
    // Act
    renderMain({ home: { ...home, recentMoments: [] } });

    // Assert
    expect(screen.getByRole('heading', { name: 'No learning overview yet' })).toBeInTheDocument();
    expect(screen.getByText('Add learning logs to build a household learning pulse.')).toBeInTheDocument();
  });
});




