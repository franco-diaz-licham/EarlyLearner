const shortDateFormatter = new Intl.DateTimeFormat('en-AU', {
  day: '2-digit',
  month: 'short',
  year: 'numeric'
});

export const formatShortDate = (value: string): string => shortDateFormatter.format(new Date(value));
