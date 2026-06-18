import LightLogo from '../../assets/logo.png';

const navigationItems = ['Home', 'Plan', 'Journal', 'Progress', 'Portfolio', 'Calendar'];

const SearchIcon = () => (
  <svg aria-hidden="true" className="h-5 w-5" fill="none" viewBox="0 0 24 24">
    <circle cx="11" cy="11" r="7" stroke="currentColor" strokeWidth="2" />
    <path d="m16.5 16.5 4 4" stroke="currentColor" strokeLinecap="round" strokeWidth="2" />
  </svg>
);

const BellIcon = () => (
  <svg aria-hidden="true" className="h-5 w-5" fill="none" viewBox="0 0 24 24">
    <path d="M18 9a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9Z" stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" />
    <path d="M10 21h4" stroke="currentColor" strokeLinecap="round" strokeWidth="2" />
  </svg>
);

const ChevronDownIcon = () => (
  <svg aria-hidden="true" className="h-4 w-4" fill="none" viewBox="0 0 24 24">
    <path d="m6 9 6 6 6-6" stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" />
  </svg>
);

export const AppTopBar = () => (
  <header className="flex min-h-16 items-center justify-between gap-4 rounded-md border border-brand-border/70 bg-white/90 px-4 shadow-app-card backdrop-blur sm:px-6">
    <div className="flex min-w-0 items-center gap-2">
      <img src={LightLogo} alt="logo" className="layout-topbar-logo-image" width={30} />
      <span className="text-lg font-extrabold text-brand-heading">EarlyLearner</span>
    </div>

    <nav aria-label="Primary" className="hidden flex-1 items-center justify-center gap-8 lg:flex">
      {navigationItems.map((item) => {
        const isActive = item === 'Home';

        return (
          <button className={`relative min-h-12 px-1 text-sm font-semibold ${isActive ? 'text-[#ef7676]' : 'text-brand-heading'}`} key={item} type="button">
            {item}
            <span className={`absolute bottom-1 left-1/2 h-1 w-1 -translate-x-1/2 rounded-full ${isActive ? 'bg-[#ef7676]' : 'bg-brand-muted'}`} />
          </button>
        );
      })}
    </nav>

    <div className="flex items-center gap-3">
      <button aria-label="Search" className="hidden h-10 w-10 items-center justify-center rounded-full text-brand-heading hover:bg-brand-surface-muted sm:flex" type="button">
        <SearchIcon />
      </button>
      <button aria-label="Notifications" className="relative hidden h-10 w-10 items-center justify-center rounded-full text-brand-heading hover:bg-brand-surface-muted sm:flex" type="button">
        <BellIcon />
        <span className="absolute right-2 top-2 h-2.5 w-2.5 rounded-full border-2 border-white bg-[#ef7676]" />
      </button>

      <button className="flex items-center gap-3 rounded-full px-1 py-1 hover:bg-brand-surface-muted" type="button">
        <span className="h-10 w-10 rounded-full bg-[linear-gradient(135deg,#f7c7b6,#f6e4a7_45%,#b9d7c5)]" />
        <span className="hidden text-left sm:block">
          <span className="block text-sm font-bold leading-4 text-brand-heading">Sophia</span>
          <span className="block text-xs leading-4 text-brand-muted">4y 2m</span>
        </span>
        <ChevronDownIcon />
      </button>
    </div>
  </header>
);
