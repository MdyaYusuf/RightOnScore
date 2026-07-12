export type ReturnModel<T> = {
  success: boolean;
  message: string | null;
  data: T | null;
  statusCode: number;
  errors: string[] | null;
};

export type UserResponseDto = {
  id: string;
  username: string;
  email: string;
  profileImageUrl: string | null;
  isActive: boolean;
  createdDate: string;
  roleId: number;
  roleName: string;
};
