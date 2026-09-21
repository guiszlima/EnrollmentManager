export interface StudentCreateDto {
  userId: number;
  cpf?: string | null;
  passportNumber?: string | null;
  nationality: string;
  birthDate: string;
  phone: string;
  address: string;
  formatIds: number[];
}

export interface UserStudentCreateDto extends Omit<StudentCreateDto, 'userId'> {
  userName: string;
  email: string;
  password: string;
}

export interface StudentUpdateDto {
  cpf?: string | null;
  passportNumber?: string | null;
  nationality?: string | null;
  birthDate: string;
  phone: string;
  address: string;
  formatIds: number[];
}

export interface StudentResponseDto {
  userId: number;
  userName: string;
  email: string;
  cpf?: string | null;
  passportNumber?: string | null;
  nationality?: string | null;
  birthDate: string;
  phone: string;
  address: string;
  registrationNumber: string;
  formatIds: number[];
}
