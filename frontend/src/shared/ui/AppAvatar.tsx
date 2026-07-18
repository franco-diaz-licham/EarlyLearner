import { UilEditAlt, UilTrashAlt, UilUsersAlt } from '@iconscout/react-unicons';
import { Button } from 'primereact/button';
import type { ChangeEvent } from 'react';
import { useId, useMemo, useRef, useState } from 'react';
import { useStoredFileContentQuery } from '../queries/fileUpload.queries';
import { mergeClassNames } from './mergeClassNames';

type AppAvatarSize = 'sm' | 'md' | 'lg';

interface AppAvatarProps {
  accept?: string;
  alt: string;
  className?: string;
  disabled?: boolean;
  initials?: string;
  readOnly?: boolean;
  src?: string | null;
  storedFileId?: string | null;
  size?: AppAvatarSize;
  onFileChange?: (file: File | null) => void;
}

const sizeClasses: Record<AppAvatarSize, string> = {
  sm: 'h-16 w-16',
  md: 'h-24 w-24',
  lg: 'h-32 w-32'
};

const iconSizeClasses: Record<AppAvatarSize, string> = {
  sm: 'h-6 w-6',
  md: 'h-8 w-8',
  lg: 'h-10 w-10'
};

const StoredFileAvatarImage = ({ alt, storedFileId }: { alt: string; storedFileId: string }) => {
  const storedFileContentQuery = useStoredFileContentQuery(storedFileId);
  const storedFileUrl = useMemo(() => (storedFileContentQuery.data ? URL.createObjectURL(storedFileContentQuery.data) : null), [storedFileContentQuery.data]);

  return storedFileUrl ? <img alt={alt} className="h-full w-full object-cover" src={storedFileUrl} /> : null;
};

export const AppAvatar = ({ accept = 'image/*', alt, className, disabled = false, initials, readOnly = false, src, storedFileId, size = 'md', onFileChange }: AppAvatarProps) => {
  const inputId = useId();
  const inputRef = useRef<HTMLInputElement>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const imageSrc = previewUrl ?? src;
  const hasImage = Boolean(imageSrc ?? storedFileId);
  const visibleInitials = initials?.trim().slice(0, 2).toUpperCase();

  const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0] ?? null;

    setPreviewUrl((currentPreviewUrl) => {
      if (currentPreviewUrl) URL.revokeObjectURL(currentPreviewUrl);
      return file ? URL.createObjectURL(file) : null;
    });

    onFileChange?.(file);
    if (previewUrl) URL.revokeObjectURL(previewUrl);
  };

  const handleClear = () => {
    setPreviewUrl((currentPreviewUrl) => {
      if (currentPreviewUrl) URL.revokeObjectURL(currentPreviewUrl);
      return null;
    });

    if (inputRef.current) inputRef.current.value = '';
    onFileChange?.(null);
  };

  return (
    <div className={mergeClassNames('inline-flex items-center gap-4', className)}>
      <div className={mergeClassNames('flex shrink-0 items-center justify-center overflow-hidden rounded-full border border-brand-border bg-brand-surface-muted text-brand-heading', sizeClasses[size])}>
        {imageSrc ? <img alt={alt} className="h-full w-full object-cover" src={imageSrc} /> : null}
        {!imageSrc && storedFileId ? <StoredFileAvatarImage alt={alt} storedFileId={storedFileId} /> : null}
        {!hasImage && visibleInitials ? <span className="text-lg font-bold">{visibleInitials}</span> : null}
        {!hasImage && !visibleInitials ? <UilUsersAlt aria-hidden="true" className={mergeClassNames('text-brand-muted', iconSizeClasses[size])} /> : null}
      </div>

      {!readOnly ? (
        <div className="flex items-center gap-2">
          <input ref={inputRef} accept={accept} className="sr-only" disabled={disabled} id={inputId} type="file" onChange={handleFileChange} />
          <label
            className={mergeClassNames(
              'inline-flex h-10 w-10 cursor-pointer items-center justify-center rounded-full border border-brand-border bg-brand-surface text-brand-heading transition',
              'focus-within:outline-2 focus-within:outline-offset-2 focus-within:outline-brand-primary hover:bg-brand-surface-muted',
              disabled && 'pointer-events-none cursor-not-allowed opacity-60'
            )}
            htmlFor={inputId}
            title="Upload image"
          >
            <UilEditAlt aria-hidden="true" className="h-5 w-5" />
            <span className="sr-only">Upload image</span>
          </label>

          {previewUrl ? (
            <Button
              aria-label="Remove selected image"
              className="inline-flex h-10 w-10 items-center justify-center rounded-full border-0 bg-transparent p-0 text-brand-error transition hover:bg-brand-primary-soft focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand-primary disabled:cursor-not-allowed disabled:opacity-60"
              disabled={disabled}
              icon={<UilTrashAlt aria-hidden="true" className="h-5 w-5" />}
              title="Remove selected image"
              type="button"
              onClick={handleClear}
            />
          ) : null}
        </div>
      ) : null}
    </div>
  );
};

