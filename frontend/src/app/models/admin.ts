export interface AdminUserDto {
  id: number;
  username: string;
  email: string;
  role: string | null;
  status: boolean;
}

export interface RoleResponseDto {
  id: number;
  name: string;
}

export interface ApproveUserDto {
  roleId: number;
}

export interface ChangeUserRoleDto {
  roleId: number;
}
