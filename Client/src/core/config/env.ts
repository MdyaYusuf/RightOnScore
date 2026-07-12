const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined;

/** Empty string uses same-origin (Vite proxy in development). */
export const API_BASE_URL = configuredBaseUrl?.replace(/\/$/, "") ?? "";

export const CLIENT_PLATFORM_HEADER = "Web";
