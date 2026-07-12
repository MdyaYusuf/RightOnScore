export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterUserRequest = {
  username: string;
  email: string;
  password: string;
};

export type AuthStatus = "idle" | "loading" | "authenticated" | "anonymous";
