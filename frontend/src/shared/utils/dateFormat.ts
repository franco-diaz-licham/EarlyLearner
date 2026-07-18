const shortDateFormatter = new Intl.DateTimeFormat('en-AU', {
  day: '2-digit',
  month: 'short',
  year: 'numeric'
});

export const formatShortDate = (value: string): string => shortDateFormatter.format(new Date(value));

const displayDateFormatter = new Intl.DateTimeFormat(undefined, {
  day: '2-digit',
  month: 'short',
  year: 'numeric'
});

export const formatDisplayDate = (value: string): string => displayDateFormatter.format(new Date(`${value}T00:00:00`));