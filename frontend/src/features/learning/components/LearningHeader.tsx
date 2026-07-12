import { UilClipboardNotes } from '@iconscout/react-unicons';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';

interface LearningHeaderProps {
  onAddLog: () => void;
}

export const LearningHeader = ({ onAddLog }: LearningHeaderProps) => {
  return (
    <AppCard>
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div>
          <p className="text-sm font-semibold text-brand-muted">Learning</p>
          <h1 id="learning-title" className="mt-1 text-3xl font-bold text-brand-heading">
            Capture the small moments
          </h1>
          <p className="mt-2 max-w-2xl text-brand-muted">Record activities, reading and routines while they are fresh, then use them to shape the next plan.</p>
        </div>
        <AppButton icon={<UilClipboardNotes aria-hidden="true" className="h-5 w-5" />} label="Add log" type="button" onClick={onAddLog} />
      </div>
    </AppCard>
  );
};
