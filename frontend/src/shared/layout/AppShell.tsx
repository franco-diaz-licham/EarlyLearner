import { UilBookOpen, UilCalendarAlt, UilClipboardNotes, UilEstate, UilShieldCheck } from "@iconscout/react-unicons";
import { NavLink, Outlet } from "react-router-dom";
import { appConfig } from "../config/appConfig";
import { useSessionStore } from "../stores/sessionStore";

const navigationItems = [
    { to: "/dashboard", label: "Dashboard", icon: UilEstate },
    { to: "/plans", label: "Plans", icon: UilClipboardNotes },
    { to: "/records", label: "Records", icon: UilBookOpen },
    { to: "/calendar", label: "Calendar", icon: UilCalendarAlt },
];

export const AppShell = () => {
    const currentUser = useSessionStore((state) => state.currentUser);

    return (
        <div className="min-h-screen bg-brand-surface">
            <aside className="fixed inset-y-0 left-0 hidden w-64 border-r border-brand-border bg-brand-white lg:block">
                <div className="flex h-16 items-center gap-3 border-b border-brand-border px-5">
                    <div className="flex h-9 w-9 items-center justify-center rounded-md bg-brand-blue-500 text-white">
                        <UilShieldCheck aria-hidden="true" size={20} />
                    </div>
                    <div>
                        <p className="text-sm font-semibold text-brand-heading">{appConfig.appName}</p>
                        <p className="text-xs text-brand-muted">Learning operations</p>
                    </div>
                </div>
                <nav aria-label="Primary" className="space-y-1 px-3 py-4">
                    {navigationItems.map((item) => {
                        const Icon = item.icon;
                        return (
                            <NavLink
                                className={({ isActive }) =>
                                    `flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium ${
                                        isActive
                                            ? "bg-brand-surface-muted text-brand-blue-700"
                                            : "text-brand-muted hover:bg-brand-surface-muted hover:text-brand-heading"
                                    }`
                                }
                                key={item.to}
                                to={item.to}
                            >
                                <Icon aria-hidden="true" size={18} />
                                {item.label}
                            </NavLink>
                        );
                    })}
                </nav>
            </aside>

            <div className="lg:pl-64">
                <header className="sticky top-0 z-10 flex min-h-16 items-center justify-between border-b border-brand-border bg-brand-white px-4 sm:px-6">
                    <div>
                        <p className="text-sm font-semibold text-brand-heading">{currentUser.organisationName}</p>
                        <p className="text-xs text-brand-muted">Workspace overview</p>
                    </div>
                    <div className="text-right">
                        <p className="text-sm font-medium text-brand-heading">{currentUser.displayName}</p>
                        <p className="text-xs text-brand-muted">{currentUser.roleLabel}</p>
                    </div>
                </header>
                <main className="mx-auto w-full max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
                    <Outlet />
                </main>
            </div>
        </div>
    );
};
