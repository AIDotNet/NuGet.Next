import MainLayout from "@/app/layout";
import Welcome from "@/app/welcome";
import Packages from "@/app/packages";
import PackageDetails from "@/app/packages/details";
import { createBrowserRouter, RouteObject } from "react-router-dom";
import LoginPage from "@/app/login";
import Upload from "@/app/upload";
import CommonHistory from "@/app/common-history";
import AdminLayout from "@/admin/layout";
import AdminWelcome from "@/admin/welcome";
import UserManagement from "@/admin/user-management";
import AdminCommonHistory from "@/admin/common-history";
import AdminSettings from "@/admin/settings";
import PackageManagementPage from "@/admin/package-management";
import KeyManager from "@/app/key-manager";
import DocsPage from "@/app/docs";
import AboutPage from "@/app/about";
import PrivacyPage from "@/app/privacy";


const routes = [
    {
        element: <MainLayout />,
        children: [
            {
                element: <Welcome />,
                path: "/",
            },
            {
                element: <Packages />,
                path: "/packages",
            },
            {
                element: <PackageDetails />,
                path: "/packages/:id/:version",
            },
            {
                element: <PackageDetails />,
                path: "/packages/:id",
            },
            {
                element: <Upload />,
                path: "/upload",
            },
            {
                element: <CommonHistory />,
                path: "/common-history",
            },
            {
                element: <KeyManager />,
                path: "/key-manager",
            },
            {
                element: <DocsPage />,
                path: "/docs",
            },
            {
                element: <AboutPage />,
                path: "/about",
            },
            {
                element: <PrivacyPage />,
                path: "/privacy",
            }
        ],
    },
    {
        element: <AdminLayout></AdminLayout>,
        children: [
            {
                path: "/admin",
                element: <AdminWelcome />
            },
            {
                path: "/admin/user-management",
                element: <UserManagement />
            },
            {
                path: "/admin/common-history",
                element: <AdminCommonHistory />
            },
            {
                path: "/admin/settings",
                element: <AdminSettings />
            },
            {
                path: "/admin/package-management",
                element: <PackageManagementPage />
            }
        ]
    },
    {
        path: "/login",
        element: <LoginPage />,
    }
] as RouteObject[];

const browserRoutes = createBrowserRouter(routes);

export default browserRoutes;