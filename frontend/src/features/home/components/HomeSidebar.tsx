import { ChildrenCard } from './ChildrenCard';
import { HouseholdPulseCard } from './HouseholdPulseCard';
import { LearningCoverageCard } from './LearningCoverageCard';
import type { HomeModel } from '../types/home.types';

interface HomeSidebarProps {
  home?: HomeModel;
  isLoading: boolean;
}

export const HomeSidebar = ({ home, isLoading }: HomeSidebarProps) => {
  const metrics = home?.metrics ?? [];
  const coverage = home?.outcomeCoverage;
  const children = home?.children ?? [];

  return (
    <div className="space-y-5">
      <LearningCoverageCard coverage={coverage} isLoading={isLoading} />
      <HouseholdPulseCard metrics={metrics} />
      <ChildrenCard children={children} />
    </div>
  );
};
