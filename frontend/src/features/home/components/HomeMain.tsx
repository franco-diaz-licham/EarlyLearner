import { AppHero } from './AppHero';
import { HomeLearningGuideCard } from './HomeLearningGuideCard';
import { HomeQuickActions } from './HomeQuickActions';
import { LearningOverviewGraph } from './LearningOverviewGraph';
import { useSessionStore } from '../../../shared/stores/sessionStore';
import type { HomeModel } from '../types/home.types';

interface HomeMainProps {
  home?: HomeModel;
  isLoading: boolean;
}

export const HomeMain = ({ home, isLoading }: HomeMainProps) => {
  const outcomeCoverage = home?.outcomeCoverage;
  const recentMoments = home?.recentMoments ?? [];
  const childName = home?.children[0]?.givenName;
  const currentUserName = useSessionStore((state) => state.currentUser.displayName);

  return (
    <div className="space-y-5">
      <AppHero childName={childName} currentUserName={currentUserName} />
      <HomeQuickActions />
      <LearningOverviewGraph isLoading={isLoading} outcomeCoverage={outcomeCoverage} recentMoments={recentMoments} />
      <HomeLearningGuideCard />
    </div>
  );
};


