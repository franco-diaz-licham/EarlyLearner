import { HomeSidebar } from '../components/HomeSidebar';
import { HomeMain } from '../components/HomeMain';
import { useHomeQuery } from '../queries/home.queries';

const formatDateInputValue = (date: Date) => date.toISOString().slice(0, 10);

export const HomePage = () => {
  const homeQuery = useHomeQuery({ today: formatDateInputValue(new Date()) });
  const home = homeQuery.data;

  return (
    <section aria-labelledby="home-title" className="space-y-5">
      <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_390px]">
        <HomeMain home={home} isLoading={homeQuery.isLoading} />
        <HomeSidebar home={home} isLoading={homeQuery.isLoading} />
      </div>
    </section>
  );
};
