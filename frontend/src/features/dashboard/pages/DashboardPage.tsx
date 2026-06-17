import { UilCalendarAlt, UilFileAlt, UilPlus } from "@iconscout/react-unicons";
import { AppButton } from "../../../shared/ui/AppButton";
import { AppStatusBadge } from "../../../shared/ui/AppStatusBadge";

const metrics = [
    { label: "Active children", value: "18", detail: "3 awaiting readiness review" },
    { label: "Plans in progress", value: "7", detail: "2 sessions scheduled today" },
    { label: "Records this week", value: "42", detail: "12 include attached evidence" },
];

const activities = [
    { child: "Ava C.", activity: "Fine motor tray", status: "Reviewed", tone: "success" },
    { child: "Noah P.", activity: "Story sequencing", status: "Draft", tone: "warning" },
    { child: "Mia R.", activity: "Outdoor counting walk", status: "Reviewed", tone: "success" },
] as const;

export const DashboardPage = () => (
    <section aria-labelledby="dashboard-title" className="space-y-6">
        <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
            <div>
                <h1 id="dashboard-title" className="text-2xl font-semibold text-brand-heading">
                    Dashboard
                </h1>
                <p className="mt-1 text-sm text-brand-muted">Daily learning operations, planning signals, and record capture.</p>
            </div>
            <div className="flex flex-wrap gap-2">
                <AppButton variant="secondary">
                    <UilCalendarAlt aria-hidden="true" size={18} />
                    Schedule
                </AppButton>
                <AppButton>
                    <UilPlus aria-hidden="true" size={18} />
                    New record
                </AppButton>
            </div>
        </div>

        <div className="grid gap-4 md:grid-cols-3">
            {metrics.map((metric) => (
                <article className="rounded-md border border-brand-border bg-brand-white p-5 shadow-app-card" key={metric.label}>
                    <p className="text-sm font-medium text-brand-muted">{metric.label}</p>
                    <p className="mt-3 text-3xl font-semibold text-brand-heading">{metric.value}</p>
                    <p className="mt-2 text-sm text-brand-muted">{metric.detail}</p>
                </article>
            ))}
        </div>

        <section className="rounded-md border border-brand-border bg-brand-white shadow-app-card">
            <div className="flex items-center justify-between border-b border-brand-border px-5 py-4">
                <div>
                    <h2 className="text-base font-semibold text-brand-heading">Recent activity records</h2>
                    <p className="text-sm text-brand-muted">Evidence and learning notes ready for follow-up.</p>
                </div>
                <UilFileAlt aria-hidden="true" className="text-brand-blue-700" size={20} />
            </div>
            <div className="overflow-x-auto">
                <table className="w-full min-w-[620px] border-collapse text-left text-sm">
                    <thead className="bg-brand-surface-muted text-xs uppercase text-brand-muted">
                        <tr>
                            <th className="px-5 py-3 font-semibold">Child</th>
                            <th className="px-5 py-3 font-semibold">Activity</th>
                            <th className="px-5 py-3 font-semibold">Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {activities.map((activity) => (
                            <tr className="border-t border-brand-border" key={`${activity.child}-${activity.activity}`}>
                                <td className="px-5 py-4 font-medium text-brand-heading">{activity.child}</td>
                                <td className="px-5 py-4 text-brand-muted">{activity.activity}</td>
                                <td className="px-5 py-4">
                                    <AppStatusBadge tone={activity.tone}>{activity.status}</AppStatusBadge>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </section>
    </section>
);
