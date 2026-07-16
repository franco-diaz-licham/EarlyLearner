import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import * as homeQueries from '../queries/home.queries';
import type { HomeModel } from '../types/home.types';
import { HomePage } from './HomePage';

vi.mock('../queries/home.queries', () => ({
  useHomeQuery: vi.fn()
}));

const home: HomeModel = {
  children: [
    {
      childId: 'child-1',
      givenName: 'Mia',
      dateOfBirth: '2021-04-15'
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

const homeQuery = {
  data: home,
  isLoading: false
};

const renderPage = () =>
  render(
    <MemoryRouter>
      <HomePage />
    </MemoryRouter>
  );

describe('HomePage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(homeQueries.useHomeQuery).mockReturnValue(homeQuery as ReturnType<typeof homeQueries.useHomeQuery>);
  });

  test('renders the learning-aware home dashboard', () => {
    // Act
    renderPage();

    // Assert
    expect(screen.getByRole('heading', { name: 'Good morning, Franco!' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Today' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Recent Learning' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Learning Coverage' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Read a story' })).toBeInTheDocument();
    expect(screen.getByText('Open invites')).toBeInTheDocument();
  });
});
