import { Link } from "react-router-dom";

export const NotFoundPage = () => (
    <section className="rounded-md border border-brand-border bg-brand-white p-8 shadow-app-card">
        <p className="text-sm font-semibold uppercase tracking-wide text-brand-blue-700">404</p>
        <h1 className="mt-2 text-2xl font-semibold text-brand-heading">Page not found</h1>
        <p className="mt-2 max-w-xl text-sm text-brand-muted">The requested workspace page does not exist or is not available yet.</p>
        <div className="mt-6">
            <Link
                className="inline-flex min-h-10 items-center justify-center rounded-md bg-brand-blue-500 px-4 text-sm font-semibold text-white transition hover:bg-brand-blue-700 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-teal"
                to="/dashboard"
            >
                Return to dashboard
            </Link>
        </div>
    </section>
);
