type StatusTone = 'success' | 'warning' | 'neutral';

interface AppStatusBadgeProps {
  children: string;
  tone?: StatusTone;
}

const toneClassNames: Record<StatusTone, string> = {
  neutral: 'bg-brand-surface-muted text-brand-text',
  success: 'bg-green-50 text-brand-success',
  warning: 'bg-amber-50 text-brand-warning'
};

export const AppStatusBadge = ({ children, tone = 'neutral' }: AppStatusBadgeProps) => <span className={`inline-flex rounded-md px-2 py-1 text-xs font-semibold ${toneClassNames[tone]}`}>{children}</span>;
