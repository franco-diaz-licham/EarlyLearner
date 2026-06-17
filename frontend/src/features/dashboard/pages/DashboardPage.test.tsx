import "@testing-library/jest-dom/vitest";
import { render, screen } from "@testing-library/react";
import { DashboardPage } from "./DashboardPage";

describe("DashboardPage", () => {
    test("renders the operational dashboard summary", () => {
        render(<DashboardPage />);

        expect(screen.getByRole("heading", { name: "Dashboard" })).toBeInTheDocument();
        expect(screen.getByText("Active children")).toBeInTheDocument();
        expect(screen.getByRole("button", { name: /new record/i })).toBeInTheDocument();
    });
});
