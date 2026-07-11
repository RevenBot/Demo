export const parseUtcDate = (value: string): Date | null => {
  if (!value) return null;
  const normalized = value.endsWith('Z') ? value : `${value}Z`;
  const d = new Date(normalized);
  return Number.isNaN(d.getTime()) ? null : d;
};

export const toUtcIso = (date: Date | null): string =>
  date ? date.toISOString() : '';

export const formatLocalDateTime = (value: string): string => {
  const d = parseUtcDate(value);
  return d ? d.toLocaleString() : value;
};
