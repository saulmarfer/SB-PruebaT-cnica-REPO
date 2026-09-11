import type { ReactNode } from "react";
import { Sidebar } from "./Sidebar";
import { Topbar } from "./Topbar";

export function AppLayout({ children }: { children: ReactNode }) {
  return (
    <div className="app-shell">
      <Sidebar />
      <div className="app-shell__contenido">
        <Topbar />
        <main className="app-shell__main">
          <div className="tarjeta">{children}</div>
        </main>
      </div>
    </div>
  );
}
