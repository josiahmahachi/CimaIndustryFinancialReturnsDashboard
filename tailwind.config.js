/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./blazor-wasm-ui/Pages/**/*.{razor,cs}",
    "./blazor-wasm-ui/Shared/**/*.{razor,cs}",
    "./blazor-wasm-ui/Components/**/*.{razor,cs}",
  ],
  theme: {
    extend: {
      colors: {
        // Primary colors
        background: "#ffffff",
        foreground: "oklch(0.145 0 0)",
        card: "#ffffff",
        "card-foreground": "oklch(0.145 0 0)",
        popover: "oklch(1 0 0)",
        "popover-foreground": "oklch(0.145 0 0)",
        primary: "#030213",
        "primary-foreground": "oklch(1 0 0)",
        secondary: "oklch(0.95 0.0058 264.53)",
        "secondary-foreground": "#030213",
        muted: "#ececf0",
        "muted-foreground": "#717182",
        accent: "#e9ebef",
        "accent-foreground": "#030213",
        destructive: "#d4183d",
        "destructive-foreground": "#ffffff",
        border: "rgba(0, 0, 0, 0.1)",
        input: "transparent",
        "input-background": "#f3f3f5",
        "switch-background": "#cbced4",
        ring: "oklch(0.708 0 0)",
        
        // Chart colors (for future use with Recharts)
        "chart-1": "oklch(0.646 0.222 41.116)",
        "chart-2": "oklch(0.6 0.118 184.704)",
        "chart-3": "oklch(0.398 0.07 227.392)",
        "chart-4": "oklch(0.828 0.189 84.429)",
        "chart-5": "oklch(0.769 0.188 70.08)",
        
        // Sidebar colors
        sidebar: "oklch(0.985 0 0)",
        "sidebar-foreground": "oklch(0.145 0 0)",
        "sidebar-primary": "#030213",
        "sidebar-primary-foreground": "oklch(0.985 0 0)",
        "sidebar-accent": "oklch(0.97 0 0)",
        "sidebar-accent-foreground": "oklch(0.205 0 0)",
        "sidebar-border": "oklch(0.922 0 0)",
        "sidebar-ring": "oklch(0.708 0 0)",
        
        // Semantic colors from Tailwind defaults (used in utilities)
        "neutral-50": "#f9fafb",
        "neutral-100": "#f3f4f6",
        "neutral-200": "#e5e7eb",
        "neutral-300": "#d1d5db",
        "neutral-400": "#9ca3af",
        "neutral-600": "#6b7280",
        "neutral-700": "#374151",
        "neutral-900": "#111827",
        
        // Status colors (from current Blazor styles)
        "status-available": "#10b981",
        "status-prepared": "#3b82f6",
        "status-ready": "#8b5cf6",
        "status-processed": "#059669",
        "status-returned": "#dc2626",
        "status-waived": "#6b7280",
        "status-outstanding": "#ea580c",
        "status-deferred": "#6366f1",
      },
      borderRadius: {
        sm: "0.375rem",
        DEFAULT: "0.5rem",
        md: "0.5rem",
        lg: "0.625rem",
      },
      fontSize: {
        xs: ["0.75rem", { lineHeight: "1rem" }],           // 12px
        sm: ["0.875rem", { lineHeight: "1.25rem" }],       // 14px
        base: ["1rem", { lineHeight: "1.5rem" }],          // 16px
        lg: ["1.125rem", { lineHeight: "1.75rem" }],       // 18px
        xl: ["1.25rem", { lineHeight: "1.75rem" }],        // 20px
        "2xl": ["1.5rem", { lineHeight: "2rem" }],         // 24px (standardized)
        "3xl": ["1.875rem", { lineHeight: "2.25rem" }],    // 30px (standardized)
      },
      fontWeight: {
        thin: 100,
        extralight: 200,
        light: 300,
        normal: 400,
        medium: 500,
        semibold: 600,
        bold: 700,
        extrabold: 800,
        black: 900,
      },
      spacing: {
        0: "0",
        0.25: "0.0625rem",
        0.5: "0.125rem",
        1: "0.25rem",
        2: "0.5rem",
        3: "0.75rem",
        4: "1rem",
        6: "1.5rem",
        8: "2rem",
        10: "2.5rem",
        12: "3rem",
        16: "4rem",
      },
      boxShadow: {
        sm: "0 1px 2px 0 rgba(0, 0, 0, 0.05)",
        DEFAULT: "0 1px 3px 0 rgba(0, 0, 0, 0.1)",
        md: "0 4px 6px -1px rgba(0, 0, 0, 0.1)",
        lg: "0 10px 15px -3px rgba(0, 0, 0, 0.1)",
      },
    },
  },
  plugins: [],
};
