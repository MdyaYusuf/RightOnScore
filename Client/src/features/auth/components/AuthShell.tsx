import type { ReactNode } from "react";

type AuthShellProps = {
  children: ReactNode;
  contentClassName?: string;
};

export function AuthShell({ children, contentClassName = "max-w-[400px]" }: AuthShellProps) {
  return (
    <div className="relative flex min-h-screen flex-col overflow-hidden bg-background text-on-surface">
      <div className="absolute inset-0 z-0">
        <div
          className="h-full w-full bg-cover bg-center opacity-40"
          style={{ backgroundImage: "url('/images/stadium-auth.png')" }}
          role="img"
          aria-label="Stadyum atmosferi"
        />
        <div className="stadium-bg absolute inset-0" />
        <div className="grain-overlay" />
      </div>

      <main className="relative z-20 flex flex-grow items-center justify-center p-margin-mobile md:p-margin-desktop">
        <div className={`flex w-full animate-[authFadeIn_480ms_ease-out] flex-col items-center ${contentClassName}`}>
          {children}
        </div>
      </main>
    </div>
  );
}
