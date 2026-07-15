import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../../core/store/hooks";
import { register } from "../authSlice";
import { AuthShell } from "../components/AuthShell";

export function RegisterPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
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
      await dispatch(register({ username, email, password })).unwrap();
      navigate("/login", { replace: true });
    } catch {
      // Errors are surfaced by the API client toast.
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthShell contentClassName="max-w-[600px]">
      <div className="mb-8 text-center md:mb-12">
        <h1 className="mb-2 font-display text-[48px] font-bold leading-[1.1] tracking-[-0.02em] text-secondary">
          RightOnScore
        </h1>
        <p className="font-body text-[18px] font-semibold leading-[1.4] text-on-surface-variant">
          Sahaya Çıkmaya Hazır Mısın?
        </p>
      </div>

      <div className="glass-card w-full rounded-[0.75rem] p-6 md:p-8">
        <form className="flex flex-col gap-6" onSubmit={handleSubmit}>
          <div>
            <label
              className="mb-2 block font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase"
              htmlFor="register-username"
            >
              Kullanıcı Adı
            </label>
            <div className="input-inset flex items-center rounded-[0.5rem] px-4 py-3 transition-colors duration-200">
              <span className="material-symbols-outlined mr-3 text-outline">person</span>
              <input
                id="register-username"
                name="username"
                type="text"
                autoComplete="username"
                required
                minLength={3}
                maxLength={50}
                value={username}
                onChange={(event) => setUsername(event.target.value)}
                placeholder="Örn: KralTaktikci"
                className="w-full border-none bg-transparent p-0 font-body text-[16px] text-on-surface placeholder:text-outline-variant focus:ring-0 focus:outline-none"
              />
            </div>
          </div>

          <div>
            <label
              className="mb-2 block font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase"
              htmlFor="register-email"
            >
              E-posta
            </label>
            <div className="input-inset flex items-center rounded-[0.5rem] px-4 py-3 transition-colors duration-200">
              <span className="material-symbols-outlined mr-3 text-outline">mail</span>
              <input
                id="register-email"
                name="email"
                type="email"
                autoComplete="email"
                required
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                placeholder="adres@ornek.com"
                className="w-full border-none bg-transparent p-0 font-body text-[16px] text-on-surface placeholder:text-outline-variant focus:ring-0 focus:outline-none"
              />
            </div>
          </div>

          <div>
            <label
              className="mb-2 block font-label text-[12px] font-medium tracking-[0.05em] text-on-surface-variant uppercase"
              htmlFor="register-password"
            >
              Şifre
            </label>
            <div className="input-inset flex items-center rounded-[0.5rem] px-4 py-3 transition-colors duration-200">
              <span className="material-symbols-outlined mr-3 text-outline">lock</span>
              <input
                id="register-password"
                name="password"
                type="password"
                autoComplete="new-password"
                required
                minLength={8}
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                placeholder="••••••••"
                className="w-full border-none bg-transparent p-0 font-body text-[16px] text-on-surface placeholder:text-outline-variant focus:ring-0 focus:outline-none"
              />
            </div>
          </div>

          <div className="mt-4 flex flex-col items-center gap-4">
            <button
              type="submit"
              disabled={isSubmitting}
              className="flex w-full items-center justify-center gap-2 rounded-[0.5rem] bg-secondary py-4 font-body text-[18px] font-semibold text-black transition-all duration-200 hover:bg-secondary-fixed active:scale-[0.98] disabled:cursor-not-allowed disabled:opacity-70"
            >
              <span>{isSubmitting ? "Kayıt yapılıyor..." : "Kayıt ol"}</span>
              {!isSubmitting && (
                <span className="material-symbols-outlined text-[20px]">arrow_forward</span>
              )}
            </button>

            <Link
              to="/login"
              className="font-body text-[16px] leading-[1.6] text-on-surface-variant transition-colors duration-200 hover:text-secondary"
            >
              Zaten hesabın var mı? Giriş yap
            </Link>
          </div>
        </form>
      </div>
    </AuthShell>
  );
}
