import { createBrowserRouter, Navigate } from "react-router";
import { AppLayout } from "../components/layout/AppLayout";
import { DevicesPage } from "../features/devices/pages/DevicesPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <AppLayout />,
    children: [
      {
        index: true,
        element: <Navigate to="/devices" replace />,
      },
      {
        path: "devices",
        element: <DevicesPage />,
      },
    ],
  },
]);