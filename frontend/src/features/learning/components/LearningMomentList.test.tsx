import { fireEvent, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import type { LearningMomentListItem } from './LearningMomentList';
import { LearningMomentList } from './LearningMomentList';

const learningMoment: LearningMomentListItem = {
  learningMomentId: 'moment-1',
  dailyLogId: 'daily-log-1',
  childId: 'child-1',
  logDate: '2026-07-16',
  kind: 'reading',
  title: 'Read a story',
  notes: 'Read a picture book and named familiar animals.',
  learningOutcomeIds: ['outcome-1']
};

describe('LearningMomentList', () => {
  const onDeleteMoment = vi.fn<(dailyLogId: string, learningMomentId: string) => void>();
  const onLoadMore = vi.fn<() => void>();
  const onSearchTermChange = vi.fn<(searchTerm: string) => void>();

  beforeEach(() => {
    onDeleteMoment.mockClear();
    onLoadMore.mockClear();
    onSearchTermChange.mockClear();
  });

  const renderList = (overrides: Partial<Parameters<typeof LearningMomentList>[0]> = {}) =>
    render(<LearningMomentList hasMore={false} isDeleting={false} isFetchingMore={false} moments={[learningMoment]} searchTerm="" onDeleteMoment={onDeleteMoment} onLoadMore={onLoadMore} onSearchTermChange={onSearchTermChange} {...overrides} />);

  test('renders learning moments', () => {
    // Act
    renderList();

    // Assert
    expect(screen.getByRole('heading', { name: 'Recent Learning' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Read a story' })).toBeInTheDocument();
    expect(screen.getByText('Read a picture book and named familiar animals.')).toBeInTheDocument();
    expect(screen.getByText('Reading')).toBeInTheDocument();
  });

  test('calls handlers for search and delete', async () => {
    // Arrange
    const user = userEvent.setup();
    renderList();

    // Act
    fireEvent.change(screen.getByLabelText('Search recent learning'), { target: { value: 'story' } });
    await user.click(screen.getByRole('button', { name: 'Delete Read a story' }));

    // Assert
    expect(onSearchTermChange).toHaveBeenLastCalledWith('story');
    expect(onDeleteMoment).toHaveBeenCalledTimes(1);
    expect(onDeleteMoment).toHaveBeenCalledWith('daily-log-1', 'moment-1');
  });

  test('renders an empty learning moment state', () => {
    // Act
    renderList({ moments: [] });

    // Assert
    expect(screen.getByText('No learning moments recorded yet.')).toBeInTheDocument();
  });

  test('loads more moments when scrolled near the bottom', () => {
    // Arrange
    renderList({ hasMore: true });
    const scrollContainer = screen.getByRole('article').parentElement as HTMLDivElement;

    Object.defineProperty(scrollContainer, 'clientHeight', { configurable: true, value: 300 });
    Object.defineProperty(scrollContainer, 'scrollHeight', { configurable: true, value: 500 });
    Object.defineProperty(scrollContainer, 'scrollTop', { configurable: true, value: 80 });

    // Act
    fireEvent.scroll(scrollContainer);

    // Assert
    expect(onLoadMore).toHaveBeenCalledTimes(1);
  });

  test('shows loading more and disables delete while operations are in progress', () => {
    // Act
    renderList({ isDeleting: true, isFetchingMore: true });

    // Assert
    expect(screen.getByText('Loading more...')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Delete Read a story' })).toBeDisabled();
  });
});
