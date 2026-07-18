import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import * as learningOutcomeQueries from '../queries/learningOutcome.queries';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';
import { LearningOutcomesPage } from './LearningOutcomesPage';

vi.mock('../queries/learningOutcome.queries', () => ({
  useCreateLearningOutcomeMutation: vi.fn(),
  useDeleteLearningOutcomeMutation: vi.fn(),
  useLearningOutcomesQuery: vi.fn(),
  useUpdateLearningOutcomeMutation: vi.fn(),
  useUpdateLearningOutcomeStatusMutation: vi.fn()
}));

const learningOutcome: LearningOutcomeModel = {
  learningOutcomeId: 'outcome-1',
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10,
  status: LearningOutcomeStatus.Active
};

const learningOutcomesQuery = {
  data: [learningOutcome]
};

const createLearningOutcomeMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const updateLearningOutcomeMutation = {
  isPending: false,
  mutateAsync: vi.fn()
};

const updateLearningOutcomeStatusMutation = {
  isPending: false,
  mutate: vi.fn(),
  variables: null
};

const deleteLearningOutcomeMutation = {
  isPending: false,
  mutate: vi.fn(),
  variables: null
};

const renderPage = () => render(<LearningOutcomesPage />);

describe('LearningOutcomesPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();

    vi.mocked(learningOutcomeQueries.useLearningOutcomesQuery).mockReturnValue(learningOutcomesQuery as ReturnType<typeof learningOutcomeQueries.useLearningOutcomesQuery>);
    vi.mocked(learningOutcomeQueries.useCreateLearningOutcomeMutation).mockReturnValue(createLearningOutcomeMutation as unknown as ReturnType<typeof learningOutcomeQueries.useCreateLearningOutcomeMutation>);
    vi.mocked(learningOutcomeQueries.useUpdateLearningOutcomeMutation).mockReturnValue(updateLearningOutcomeMutation as unknown as ReturnType<typeof learningOutcomeQueries.useUpdateLearningOutcomeMutation>);
    vi.mocked(learningOutcomeQueries.useUpdateLearningOutcomeStatusMutation).mockReturnValue(updateLearningOutcomeStatusMutation as unknown as ReturnType<typeof learningOutcomeQueries.useUpdateLearningOutcomeStatusMutation>);
    vi.mocked(learningOutcomeQueries.useDeleteLearningOutcomeMutation).mockReturnValue(deleteLearningOutcomeMutation as unknown as ReturnType<typeof learningOutcomeQueries.useDeleteLearningOutcomeMutation>);

    createLearningOutcomeMutation.mutateAsync.mockResolvedValue(learningOutcome);
    updateLearningOutcomeMutation.mutateAsync.mockResolvedValue(learningOutcome);
  });

  test('renders the learning outcomes page from query data', () => {
    // Act
    renderPage();

    // Assert
    expect(screen.getByRole('heading', { name: 'Manage outcomes' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Learning Outcomes' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Listens and responds' })).toBeInTheDocument();
  });

  test('opens the add learning outcome form and submits an outcome', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Add' }));
    await user.type(screen.getByLabelText(/code/i), 'social-turn-taking');
    await user.type(screen.getByLabelText(/name/i), 'Takes turns');
    await user.type(screen.getByLabelText(/category/i), 'Social');
    await user.clear(screen.getByLabelText(/order/i));
    await user.type(screen.getByLabelText(/order/i), '20');
    await user.type(screen.getByLabelText(/description/i), 'Waits, shares, and takes turns with support.');
    await user.click(screen.getByRole('button', { name: 'Add outcome' }));

    // Assert
    await waitFor(() => {
      expect(createLearningOutcomeMutation.mutateAsync).toHaveBeenCalledTimes(1);
    });
    expect(createLearningOutcomeMutation.mutateAsync).toHaveBeenCalledWith({
      code: 'social-turn-taking',
      name: 'Takes turns',
      description: 'Waits, shares, and takes turns with support.',
      category: 'Social',
      sortOrder: 20
    });
  });

  test('edits a learning outcome and submits the update request', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Edit Listens and responds' }));
    await user.clear(screen.getByLabelText(/name/i));
    await user.type(screen.getByLabelText(/name/i), 'Updated outcome');
    await user.click(screen.getByRole('button', { name: 'Save outcome' }));

    // Assert
    await waitFor(() => {
      expect(updateLearningOutcomeMutation.mutateAsync).toHaveBeenCalledTimes(1);
    });
    expect(updateLearningOutcomeMutation.mutateAsync).toHaveBeenCalledWith({
      learningOutcomeId: 'outcome-1',
      form: {
        code: 'language-listening',
        name: 'Updated outcome',
        description: 'Responds to familiar sounds, words, and stories.',
        category: 'Language',
        sortOrder: 10
      }
    });
  });

  test('deletes learning outcomes', async () => {
    // Arrange
    const user = userEvent.setup();

    renderPage();

    // Act
    await user.click(screen.getByRole('button', { name: 'Delete Listens and responds' }));

    // Assert
    expect(deleteLearningOutcomeMutation.mutate).toHaveBeenCalledWith('outcome-1');
  });
});
