import { AppHero } from './AppHero';
import { HomeLearningGuideCard } from './HomeLearningGuideCard';
import { LearningOverviewGraph } from './LearningOverviewGraph';
import { LearningSummaryCard } from './LearningSummaryCard';
import type { HomeModel } from '../types/home.types';

interface HomeMainProps {
  home?: HomeModel;
  isLoading: boolean;
}

export const HomeMain = ({ home, isLoading }: HomeMainProps) => {
  const today = home?.today;
  const outcomeCoverage = home?.outcomeCoverage;
  const recentMoments = home?.recentMoments ?? [];

  return (
    <div className="space-y-5">
      <AppHero />
      <LearningSummaryCard isLoading={isLoading} today={today} />
      <LearningOverviewGraph isLoading={isLoading} outcomeCoverage={outcomeCoverage} recentMoments={recentMoments} />
      <HomeLearningGuideCard />
    </div>
  );
};
