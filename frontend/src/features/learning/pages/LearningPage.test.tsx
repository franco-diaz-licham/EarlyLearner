import { fireEvent, render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { HouseholdModel } from '../../households/types/household.types';
import * as householdQueries from '../../households/queries/household.queries';
import * as dailyLogQueries from '../queries/dailyLog.queries';
import * as learningOutcomeQueries from '../queries/learningOutcome.queries';
import type { DailyLogModel, LearningMomentFeedModel } from '../types/dailyLog.types';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';
import { LearningPage } from './LearningPage';

vi.mock('../../households/queries/household.queries', () => ({
  useHouseholdQuery: vi.fn()
}));

vi.mock('../queries/dailyLog.queries', () => ({
  useCreateDailyLogMutation: vi.fn(),
  useDailyLogsQuery: vi.fn(),
  useDeleteLearningMomentMutation: vi.fn(),
  useUpdateLearningMomentMutation: vi.fn(),
  useLearningMomentFeedQuery: vi.fn()
}));

vi.mock('../queries/learningOutcome.queries', () => ({
  useLearningOutcomesQuery: vi.fn()
}));

const child = {
  id: 'child-1',
  firstName: 'Mia',
  lastName: 'Rivera',
  dateOfBirth: '2021-04-15',
  avatarStoredFileId: null
};

const household: HouseholdModel = {
  id: 'household-1',
  name: 'Rivera Household',
  carers: [],
  children: [child],
  invitations: []
};

const learningOutcome: LearningOutcomeModel = {
  learningOutcomeId: 'outcome-1',
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10,
  status: LearningOutcomeStatus.Active
};

const dailyLog: DailyLogModel = {
  dailyLogId: 'daily-log-1',
  householdId: 'household-1',
  childId: 'child-1',
  logDate: new Date().toISOString().slice(0, 10),
  learningMomentCount: 3,
  learningMoments: [
    {
      learningMomentId: 'moment-activity',
      kind: 'activity',
      title: 'Paint mixing',
      notes: 'Mixed colours and described the changes.',
      learningOutcomeIds: ['outcome-1']
    },
    {
      learningMomentId: 'moment-reading',
      kind: 'reading',
      title: 'Read a story',
      notes: 'Read a picture book and named familiar animals.',
      learningOutcomeIds: ['outcome-1']
    },
    {
      learningMomentId: 'moment-routine',
      kind: 'routine',
      title: 'Packed away',
      notes: 'Followed the pack-away routine.',
      learningOutcomeIds: ['outcome-1']
    }
  ]
};

const learningMoment: LearningMomentFeedModel = {
  learningMomentId: 'moment-reading',
  dailyLogId: 'daily-log-1',
  householdId: 'household-1',
  childId: 'child-1',
  logDate: dailyLog.logDate,
  kind: 'reading',
  title: 'Read a story',
  notes: 'Read a picture book and named familiar animals.',
  learningOutcomeIds: ['outcome-1']
};

const householdQuery = {
  data: household
};

const learningOutcomesQuery = {
  data: [learningOutcome]
};

const dailyLogsQuery = {
  data: [dailyLog]
};

const learningMomentFeedQuery = {
  data: {
    pages: [
      {
        items: [learningMoment],
        pagination: {
          pageNumber: 1,
          totalPages: 2,
          pageSize: 10,
          totalCount: 11
        }
      }
    ],
    pageParams: [1]
  },
  fetchNextPage: vi.fn(),
  hasNextPage: true,
  isFetchingNextPage: false
};

const createDailyLogMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const updateLearningMomentMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const deleteLearningMomentMutation = {
  isPending: false,
  mutate: vi.fn()
};

const renderPage = () => render(<LearningPage />);

describe('LearningPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();

    vi.mocked(householdQueries.useHouseholdQuery).mockReturnValue(householdQuery as ReturnType<typeof householdQueries.useHouseholdQuery>);
    vi.mocked(learningOutcomeQueries.useLearningOutcomesQuery).mockReturnValue(learningOutcomesQuery as ReturnType<typeof learningOutcomeQueries.useLearningOutcomesQuery>);
    vi.mocked(dailyLogQueries.useDailyLogsQuery).mockReturnValue(dailyLogsQuery as ReturnType<typeof dailyLogQueries.useDailyLogsQuery>);
    vi.mocked(dailyLogQueries.useLearningMomentFeedQuery).mockReturnValue(learningMomentFeedQuery as unknown as ReturnType<typeof dailyLogQueries.useLearningMomentFeedQuery>);
    vi.mocked(dailyLogQueries.useCreateDailyLogMutation).mockReturnValue(createDailyLogMutation as unknown as ReturnType<typeof dailyLogQueries.useCreateDailyLogMutation>);
    vi.mocked(dailyLogQueries.useUpdateLearningMomentMutation).mockReturnValue(updateLearningMomentMutation as unknown as ReturnType<typeof dailyLogQueries.useUpdateLearningMomentMutation>);
    vi.mocked(dailyLogQueries.useDeleteLearningMomentMutation).mockReturnValue(deleteLearningMomentMutation as unknown as ReturnType<typeof dailyLogQueries.useDeleteLearningMomentMutation>);

    createDailyLogMutation.mutateAsync.mockResolvedValue(dailyLog);
    updateLearningMomentMutation.mutateAsync.mockResolvedValue(dailyLog);
  });

  test('renders the learning dashboard from query data', () => {
    // Act
    renderPage();

    // Assert
    expect(screen.getByRole('heading', { name: 'Capture the small moments' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Recent Learning' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Read a story' })).toBeInTheDocument();
    expect(screen.getByText('Mia Rivera')).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Today at a Glance' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Children' })).toBeInTheDocument();
    expect(screen.getByText('Mia')).toBeInTheDocument();
    expect(screen.getByText('Activities')).toBeInTheDocument();
    expect(screen.getAllByText('Reading').length).toBeGreaterThan(0);
    expect(screen.getByText('Routine')).toBeInTheDocument();
    expect(screen.queryByRole('heading', { name: 'Learning Outcomes' })).not.toBeInTheDocument();
  });

  test('opens the add learning log form and submits a log', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Add log' }));
    const dialog = screen.getByRole('dialog', { name: 'Add learning log' });
    await user.type(screen.getByLabelText(/title/i), 'Paint mixing');
    await user.type(screen.getByLabelText(/notes/i), 'Mixed colours and described the changes.');
    await user.click(within(dialog).getByLabelText(/listens and responds/i));
    await user.click(within(dialog).getByRole('button', { name: 'Add log' }));

    // Assert
    await waitFor(() => {
      expect(createDailyLogMutation.mutateAsync).toHaveBeenCalledTimes(1);
    });
    expect(createDailyLogMutation.mutateAsync).toHaveBeenCalledWith({
      childId: 'child-1',
      logDate: new Date().toISOString().slice(0, 10),
      kind: 'activity',
      title: 'Paint mixing',
      notes: 'Mixed colours and described the changes.',
      learningOutcomeIds: ['outcome-1']
    });
  });

  test('opens the edit learning log form and submits an update', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Edit Read a story' }));
    const dialog = screen.getByRole('dialog', { name: 'Edit learning log' });
    await user.clear(within(dialog).getByLabelText(/title/i));
    await user.type(within(dialog).getByLabelText(/title/i), 'Updated story');
    await user.clear(within(dialog).getByLabelText(/notes/i));
    await user.type(within(dialog).getByLabelText(/notes/i), 'Updated notes.');
    await user.click(within(dialog).getByRole('button', { name: 'Save log' }));

    // Assert
    await waitFor(() => {
      expect(updateLearningMomentMutation.mutateAsync).toHaveBeenCalledTimes(1);
    });
    expect(updateLearningMomentMutation.mutateAsync).toHaveBeenCalledWith({
      dailyLogId: 'daily-log-1',
      learningMomentId: 'moment-reading',
      form: {
        childId: 'child-1',
        logDate: dailyLog.logDate,
        kind: 'reading',
        title: 'Updated story',
        notes: 'Updated notes.',
        learningOutcomeIds: ['outcome-1']
      }
    });
  });

  test('deletes moments', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Delete Read a story' }));

    // Assert
    expect(deleteLearningMomentMutation.mutate).toHaveBeenCalledWith({
      dailyLogId: 'daily-log-1',
      learningMomentId: 'moment-reading'
    });
  });
  test('filters learning moments by selected child', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: /Mia/i }));

    // Assert
    await waitFor(() => {
      expect(dailyLogQueries.useLearningMomentFeedQuery).toHaveBeenLastCalledWith({
        childId: 'child-1',
        pageSize: 10,
        searchTerm: null
      });
    });
  });


  test('updates search term and loads more learning moments', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.type(screen.getByLabelText('Search recent learning'), 'story');
    const scrollContainer = screen.getByRole('article').parentElement as HTMLDivElement;
    Object.defineProperty(scrollContainer, 'clientHeight', { configurable: true, value: 300 });
    Object.defineProperty(scrollContainer, 'scrollHeight', { configurable: true, value: 500 });
    Object.defineProperty(scrollContainer, 'scrollTop', { configurable: true, value: 80 });
    fireEvent.scroll(scrollContainer);

    // Assert
    expect(dailyLogQueries.useLearningMomentFeedQuery).toHaveBeenLastCalledWith({
      childId: null,
      pageSize: 10,
      searchTerm: 'story'
    });
    expect(learningMomentFeedQuery.fetchNextPage).toHaveBeenCalledTimes(1);
  });
});
