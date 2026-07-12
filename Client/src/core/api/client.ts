import { toast } from "react-toastify";
import { API_BASE_URL, CLIENT_PLATFORM_HEADER } from "../config/env";
import type { ReturnModel } from "../types/api";

export type ApiRequestOptions = {
  method?: string;
  body?: unknown;
  headers?: Record<string, string>;
  /** Skip 401 → refresh → retry (used by refresh itself). */
  skipAuthRefresh?: boolean;
  /** Do not show error toasts. */
  silent?: boolean;
  /** Show success toast using API message (or a custom string). */
  successToast?: boolean | string;
};

type QueueItem = {
  resolve: (value: boolean) => void;
};

let isRefreshing = false;
const refreshQueue: QueueItem[] = [];

function buildUrl(path: string): string {
  if (path.startsWith("http://") || path.startsWith("https://")) {
    return path;
  }

  const normalizedPath = path.startsWith("/") ? path : `/${path}`;
  return `${API_BASE_URL}${normalizedPath}`;
}

function resolveQueue(success: boolean): void {
  while (refreshQueue.length > 0) {
    refreshQueue.shift()?.resolve(success);
  }
}

async function parseReturnModel<T>(response: Response): Promise<ReturnModel<T>> {
  const contentType = response.headers.get("content-type") ?? "";

  if (!contentType.includes("application/json")) {
    return {
      success: false,
      message: response.statusText || "Beklenmeyen sunucu yanıtı.",
      data: null,
      statusCode: response.status,
      errors: null,
    };
  }

  const payload = (await response.json()) as Partial<ReturnModel<T>>;

  return {
    success: payload.success ?? response.ok,
    message: payload.message ?? null,
    data: (payload.data ?? null) as T | null,
    statusCode: payload.statusCode ?? response.status,
    errors: payload.errors ?? null,
  };
}

async function refreshAccessToken(): Promise<boolean> {
  const response = await fetch(buildUrl("/api/authentication/refresh-token"), {
    method: "POST",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      "X-Client-Platform": CLIENT_PLATFORM_HEADER,
    },
    body: JSON.stringify({}),
  });

  const result = await parseReturnModel<unknown>(response);
  return response.ok && result.success;
}

async function handleUnauthorized(): Promise<boolean> {
  if (isRefreshing) {
    return new Promise<boolean>((resolve) => {
      refreshQueue.push({ resolve });
    });
  }

  isRefreshing = true;

  try {
    const refreshed = await refreshAccessToken();
    resolveQueue(refreshed);
    return refreshed;
  } catch {
    resolveQueue(false);
    return false;
  } finally {
    isRefreshing = false;
  }
}

function showErrorToast(result: ReturnModel<unknown>): void {
  if (result.errors && result.errors.length > 0) {
    toast.error(result.errors.join("\n"));
    return;
  }

  if (result.message) {
    toast.error(result.message);
  }
}

function showSuccessToast(result: ReturnModel<unknown>, successToast: boolean | string): void {
  if (typeof successToast === "string") {
    toast.success(successToast);
    return;
  }

  if (result.message) {
    toast.success(result.message);
  }
}

export async function apiClient<T>(
  path: string,
  options: ApiRequestOptions = {},
): Promise<ReturnModel<T>> {
  const {
    method = "GET",
    body,
    headers = {},
    skipAuthRefresh = false,
    silent = false,
    successToast = false,
  } = options;

  const requestHeaders: Record<string, string> = {
    "X-Client-Platform": CLIENT_PLATFORM_HEADER,
    ...headers,
  };

  let requestBody: BodyInit | undefined;

  if (body !== undefined && body !== null) {
    if (body instanceof FormData) {
      requestBody = body;
    } else {
      requestHeaders["Content-Type"] = requestHeaders["Content-Type"] ?? "application/json";
      requestBody = JSON.stringify(body);
    }
  }

  const execute = async (): Promise<ReturnModel<T>> => {
    const response = await fetch(buildUrl(path), {
      method,
      credentials: "include",
      headers: requestHeaders,
      body: requestBody,
    });

    return parseReturnModel<T>(response);
  };

  try {
    let result = await execute();

    if (!skipAuthRefresh && result.statusCode === 401) {
      const refreshed = await handleUnauthorized();

      if (refreshed) {
        result = await execute();
      }
    }

    if (result.success) {
      if (successToast) {
        showSuccessToast(result, successToast);
      }
    } else if (!silent) {
      showErrorToast(result);
    }

    return result;
  } catch {
    const networkFailure: ReturnModel<T> = {
      success: false,
      message: "Sunucuya bağlanılamadı.",
      data: null,
      statusCode: 0,
      errors: null,
    };

    if (!silent) {
      showErrorToast(networkFailure);
    }

    return networkFailure;
  }
}
