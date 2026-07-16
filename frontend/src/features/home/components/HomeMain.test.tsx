import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
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
  test('renders today metrics and recent moments', () => {
    // Act
    renderMain();

    // Assert
    expect(screen.getByRole('heading', { name: 'Today' })).toBeInTheDocument();
    expect(screen.getByText('Daily logs')).toBeInTheDocument();
    expect(screen.getByText('Learning moments')).toBeInTheDocument();
    expect(screen.getByText('Children observed')).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Recent Learning' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Read a story' })).toBeInTheDocument();
    expect(screen.getByText('Listens and responds')).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Add log' })).toHaveAttribute('href', '/learning');
    expect(screen.getByRole('link', { name: 'View all' })).toHaveAttribute('href', '/learning');
  });

  test('renders loading placeholders', () => {
    // Act
    renderMain({ home: undefined, isLoading: true });

    // Assert
    expect(screen.getAllByText('-')).toHaveLength(3);
  });

  test('renders an empty recent learning state', () => {
    // Act
    renderMain({ home: { ...home, recentMoments: [] } });

    // Assert
    expect(screen.getByRole('heading', { name: 'No learning moments yet' })).toBeInTheDocument();
    expect(screen.getByText('Add the first learning log when something worth remembering happens.')).toBeInTheDocument();
  });
});
