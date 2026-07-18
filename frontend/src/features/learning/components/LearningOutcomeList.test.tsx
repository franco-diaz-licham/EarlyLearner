import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { LearningOutcomeStatus, type LearningOutcomeModel } from '../types/learningOutcome.types';
import { LearningOutcomeList } from './LearningOutcomeList';

const learningOutcome: LearningOutcomeModel = {
  learningOutcomeId: 'outcome-1',
  code: 'language-listening',
  name: 'Listens and responds',
  description: 'Responds to familiar sounds, words, and stories.',
  category: 'Language',
  sortOrder: 10,
  status: LearningOutcomeStatus.Active
};

describe('LearningOutcomeList', () => {
  const onDeleteOutcome = vi.fn<(outcome: LearningOutcomeModel) => void>();
  const onEditOutcome = vi.fn<(outcome: LearningOutcomeModel) => void>();
  const onStatusChange = vi.fn<(outcome: LearningOutcomeModel, status: LearningOutcomeModel['status']) => void>();

  beforeEach(() => {
    onDeleteOutcome.mockClear();
    onEditOutcome.mockClear();
    onStatusChange.mockClear();
  });

  const renderList = (overrides: Partial<Parameters<typeof LearningOutcomeList>[0]> = {}) =>
    render(<LearningOutcomeList outcomes={[learningOutcome]} onDeleteOutcome={onDeleteOutcome} onEditOutcome={onEditOutcome} onStatusChange={onStatusChange} {...overrides} />);

  test('renders learning outcomes and the configured count', () => {
    // Act
    renderList();

    // Assert
    expect(screen.getByRole('heading', { name: 'Learning Outcomes' })).toBeInTheDocument();
    expect(screen.getByText('1 configured')).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Listens and responds' })).toBeInTheDocument();
    expect(screen.getByText('Language')).toBeInTheDocument();
    expect(screen.getAllByText('Active').length).toBeGreaterThan(0);
  });

  test('calls action handlers with the selected learning outcome', async () => {
    // Arrange
    const user = userEvent.setup();
    renderList();

    // Act
    await user.click(screen.getByRole('button', { name: 'Edit Listens and responds' }));
    await user.click(screen.getByRole('button', { name: 'Delete Listens and responds' }));

    // Assert
    expect(onEditOutcome).toHaveBeenCalledWith(learningOutcome);
    expect(onDeleteOutcome).toHaveBeenCalledWith(learningOutcome);
  });

  test('renders an empty learning outcome state', () => {
    // Act
    renderList({ outcomes: [] });

    // Assert
    expect(screen.getByText('0 configured')).toBeInTheDocument();
    expect(screen.getByText('No learning outcomes configured yet.')).toBeInTheDocument();
  });

  test('disables actions while matching operations are in progress', () => {
    // Act
    const { container } = renderList({ deletingId: 'outcome-1', updatingStatusId: 'outcome-1' });

    // Assert
    expect(container.querySelector('[data-p-disabled="true"]')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Delete Listens and responds' })).toBeDisabled();
  });
});
