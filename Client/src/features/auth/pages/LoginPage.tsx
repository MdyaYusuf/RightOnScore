import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../../core/store/hooks";
import { login } from "../authSlice";
import { AuthShell } from "../components/AuthShell";

export function LoginPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (isSubmitting) {
      return;
    }

    setIsSubmitting(true);

    try {
      await dispatch(login({ email, password })).unwrap();
      navigate("/seasons", { replace: true });
    } catch {
      // Errors are surfaced by the API client toast.
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthShell>
      <div className="mb-10 text-center md:mb-12">
        <h1 className="mb-2 font-display text-[48px] font-bold leading-[1.1] tracking-[-0.02em] text-secondary drop-shadow-lg">
          RightOnScore
        </h1>
        <p className="mx-auto max-w-[280px] font-body text-[18px] font-semibold leading-[1.4] text-on-surface-variant">
          Skorları tahmin et, zirveye tırman.
        </p>
      </div>

      <div className="glass-panel relative w-full overflow-hidden rounded-[0.75rem] p-6 shadow-2xl md:p-8">
        <div className="absolute top-0 right-0 left-0 h-px bg-gradient-to-r from-transparent via-white/20 to-transparent" />

        <form className="flex flex-col space-y-6" onSubmit={handleSubmit}>
          <div className="space-y-2">
            <label
              className="block font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase"
              htmlFor="login-email"
            >
              Email
            </label>
            <div className="relative">
              <span className="material-symbols-outlined absolute top-1/2 left-3 -translate-y-1/2 text-outline">
                mail
              </span>
              <input
                id="login-email"
                name="email"
                type="email"
                autoComplete="email"
                required
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                placeholder="adiniz@ornek.com"
                className="w-full rounded-[0.5rem] border border-outline-variant/50 bg-surface-container-lowest py-3 pr-4 pl-10 text-on-surface transition-colors duration-200 placeholder:text-outline focus:border-secondary focus:ring-1 focus:ring-secondary focus:outline-none"
              />
            </div>
          </div>

          <div className="space-y-2">
            <div className="flex items-center justify-between">
              <label
                className="block font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase"
                htmlFor="login-password"
              >
                Şifre
              </label>
              <span className="font-label text-[12px] font-medium tracking-[0.05em] text-primary">
                Şifremi unuttum?
              </span>
            </div>
            <div className="relative">
              <span className="material-symbols-outlined absolute top-1/2 left-3 -translate-y-1/2 text-outline">
                lock
              </span>
              <input
                id="login-password"
                name="password"
                type="password"
                autoComplete="current-password"
                required
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                placeholder="••••••••"
                className="w-full rounded-[0.5rem] border border-outline-variant/50 bg-surface-container-lowest py-3 pr-4 pl-10 text-on-surface transition-colors duration-200 placeholder:text-outline focus:border-secondary focus:ring-1 focus:ring-secondary focus:outline-none"
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="mt-2 w-full rounded-[0.5rem] bg-secondary px-4 py-3 font-body text-[18px] font-semibold text-black transition-all duration-200 hover:brightness-110 active:scale-[0.98] disabled:cursor-not-allowed disabled:opacity-70"
          >
            {isSubmitting ? "Giriş yapılıyor..." : "Giriş yap"}
          </button>
        </form>

        <div className="mt-8 border-t border-outline-variant/20 pt-6 text-center">
          <p className="font-body text-[16px] leading-[1.6] text-on-surface-variant">
            Hesabın yok mu?{" "}
            <Link
              to="/register"
              className="font-semibold text-secondary underline-offset-4 transition-all duration-200 hover:underline decoration-2"
            >
              Kayıt ol
            </Link>
          </p>
        </div>
      </div>
    </AuthShell>
  );
}
