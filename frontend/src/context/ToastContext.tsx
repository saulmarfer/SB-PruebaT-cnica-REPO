import { createContext, useCallback, useContext, useState, type ReactNode } from "react";

type TipoToast = "exito" | "error";

interface Toast {
  id: number;
  tipo: TipoToast;
  mensaje: string;
}

interface ToastContextValue {
  notificarExito: (mensaje: string) => void;
  notificarError: (mensaje: string) => void;
}

const ToastContext = createContext<ToastContextValue | undefined>(undefined);

let contador = 0;

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const quitar = useCallback((id: number) => {
    setToasts((actuales) => actuales.filter((t) => t.id !== id));
  }, []);

  const agregar = useCallback(
    (tipo: TipoToast, mensaje: string) => {
      const id = ++contador;
      setToasts((actuales) => [...actuales, { id, tipo, mensaje }]);
      setTimeout(() => quitar(id), 4500);
    },
    [quitar]
  );

  const value: ToastContextValue = {
    notificarExito: (mensaje) => agregar("exito", mensaje),
    notificarError: (mensaje) => agregar("error", mensaje),
  };

  return (
    <ToastContext.Provider value={value}>
      {children}
      <div className="toast-contenedor" role="status" aria-live="polite">
        {toasts.map((t) => (
          <div key={t.id} className={`toast toast--${t.tipo}`}>
            {t.mensaje}
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast(): ToastContextValue {
  const context = useContext(ToastContext);
  if (!context) throw new Error("useToast debe usarse dentro de <ToastProvider>");
  return context;
}
