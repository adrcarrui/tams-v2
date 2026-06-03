export type AssetVariantDto = {
  id: number;
  code: string;
  name: string;
  description: string | null;
  isActive: boolean;
  sortOrder: number;
};

export type AssetTypeDto = {
  id: number;
  code: string;
  name: string;
  description: string | null;
  managedByDepartmentId: number;
  managedByDepartmentCode: string;
  identifierPolicy: string;
  canBeAssignedToCourse: boolean;
  isReturnable: boolean;
  showInCalendar: boolean;
  isActive: boolean;
  sortOrder: number;
  icon: string | null;
  color: string | null;
  variants: AssetVariantDto[];
};