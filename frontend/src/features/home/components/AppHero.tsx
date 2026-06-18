import homeHero from '../../../assets/home-hero.png';

export const AppHero = () => {
  return (
    <section className="relative min-h-70 overflow-hidden rounded-md bg-white shadow-app-card">
      <img alt="" className="absolute inset-0 h-full w-full object-cover" src={homeHero} />
      <div className="absolute inset-0 bg-linear-to-r from-white via-white/80 to-white/5" />
      <div className="relative max-w-xl px-6 py-10 sm:px-10">
        <h1 id="home-title" className="text-4xl font-bold leading-tight text-brand-heading sm:text-5xl">
          Good morning, Franco!
        </h1>
        <p className="mt-4 max-w-sm text-lg leading-8 text-brand-muted">Sophia is curious, creative and growing every day.</p>
      </div>
    </section>
  );
};
