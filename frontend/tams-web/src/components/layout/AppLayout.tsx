import { NavLink, Outlet } from "react-router";

export function AppLayout() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">T</span>
          <div>
            <strong>TAMS v2</strong>
            <small>Training Assets Management System</small>
          </div>
        </div>

        <nav className="nav-menu">
          <NavLink to="/devices">Devices</NavLink>
        </nav>
      </aside>

      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}