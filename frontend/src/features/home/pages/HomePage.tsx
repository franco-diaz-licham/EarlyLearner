import { HomeSidebar } from '../components/HomeSidebar';
import { HomeMain } from '../components/HomeMain';

export const HomePage = () => (
  <section aria-labelledby="home-title" className="space-y-5">
    <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_390px]">
      <HomeMain />
      <HomeSidebar />
    </div>
  </section>
);
