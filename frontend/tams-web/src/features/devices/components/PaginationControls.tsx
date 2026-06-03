type PaginationControlsProps = {
  page: number;
  totalPages: number;
  totalItems: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  onPageChange: (page: number) => void;
};

export function PaginationControls({
  page,
  totalPages,
  totalItems,
  hasPreviousPage,
  hasNextPage,
  onPageChange,
}: PaginationControlsProps) {
  return (
    <div className="pagination-bar">
      <span>
        Page {page} of {totalPages || 1} · Total devices: {totalItems}
      </span>

      <div className="pagination-actions">
        <button
          type="button"
          disabled={!hasPreviousPage}
          onClick={() => onPageChange(page - 1)}
        >
          Previous
        </button>

        <button
          type="button"
          disabled={!hasNextPage}
          onClick={() => onPageChange(page + 1)}
        >
          Next
        </button>
      </div>
    </div>
  );
}