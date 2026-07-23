import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import type { HomeOutcomeCoverageModel, HomeRecentMomentModel } from '../types/home.types';
import { LearningOverviewGraph } from './LearningOverviewGraph';

const outcomeCoverage: HomeOutcomeCoverageModel = {
  activeOutcomeCount: 10,
  touchedThisWeekCount: 6,
  untouchedActiveOutcomeCount: 4
};

const recentMoments: HomeRecentMomentModel[] = [
  {
    childId: 'child-1',
    childName: 'Mia',
    dailyLogId: 'daily-log-1',
    kind: 'activity',
    learningMomentId: 'moment-1',
    logDate: '2026-07-16',
    notes: 'Built a tower.',
    outcomeNames: ['Takes turns with others'],
    title: 'Block play'
  },
  {
    childId: 'child-1',
    childName: 'Mia',
    dailyLogId: 'daily-log-2',
    kind: 'reading',
    learningMomentId: 'moment-2',
    logDate: '2026-07-16',
    notes: 'Read a story.',
    outcomeNames: ['Listens and responds'],
    title: 'Story time'
  },
  {
    childId: 'child-2',
    childName: 'Leo',
    dailyLogId: 'daily-log-3',
    kind: 'reading',
    learningMomentId: 'moment-3',
    logDate: '2026-07-17',
    notes: 'Named animals.',
    outcomeNames: ['Listens and responds'],
    title: 'Animal book'
  }
];

const renderOverviewGraph = (overrides: Partial<Parameters<typeof LearningOverviewGraph>[0]> = {}) =>
  render(
    <MemoryRouter>
      <LearningOverviewGraph isLoading={false} outcomeCoverage={outcomeCoverage} recentMoments={recentMoments} {...overrides} />
    </MemoryRouter>
  );

describe('LearningOverviewGraph', () => {
  test('renders learning mix and outcome coverage', () => {
    // Act
    const { container } = renderOverviewGraph();

    // Assert
    expect(screen.getByRole('heading', { name: 'Learning Overview' })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'View all' })).toHaveAttribute('href', '/learning');
    expect(screen.getByRole('heading', { name: 'Learning Mix' })).toBeInTheDocument();
    expect(screen.getByText('3 moments')).toBeInTheDocument();
    expect(screen.getByText('Activity')).toBeInTheDocument();
    expect(screen.getAllByText('Reading')).toHaveLength(1);
    expect(screen.getByText('Routine')).toBeInTheDocument();
    expect(screen.getByText('Used this week')).toBeInTheDocument();
    expect(screen.getByText('Not used')).toBeInTheDocument();
    expect(screen.getByText('10 active outcomes')).toBeInTheDocument();

    const bars = container.querySelectorAll('[style]');
    expect(bars[0]).toHaveStyle({ width: '33%' });
    expect(bars[2]).toHaveStyle({ width: '67%' });
  });

  test('renders loading placeholders for outcome totals', () => {
    // Act
    renderOverviewGraph({ isLoading: true, outcomeCoverage: undefined });

    // Assert
    expect(screen.getAllByText('-')).toHaveLength(2);
    expect(screen.getByText('0 active outcomes')).toBeInTheDocument();
  });

  test('renders an empty learning overview state', () => {
    // Act
    renderOverviewGraph({ recentMoments: [] });

    // Assert
    expect(screen.getByRole('heading', { name: 'No learning overview yet' })).toBeInTheDocument();
    expect(screen.getByText('Add learning logs to build a household learning pulse.')).toBeInTheDocument();
  });
});
