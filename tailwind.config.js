/** @type {import('tailwindcss').Config} */
export default {
  darkMode: "class",
  content: [
    "./blazor-wasm-ui/Pages/**/*.{razor,cs}",
    "./blazor-wasm-ui/Shared/**/*.{razor,cs}",
    "./blazor-wasm-ui/Components/**/*.{razor,cs}",
  ],
  theme: {
    extend: {
      colors: {
        // Semantic tokens using CSS variables
        background: "var(--background)",
        foreground: "var(--foreground)",
        
        card: {
          DEFAULT: "var(--card)",
          foreground: "var(--card-foreground)",
        },
        
        popover: {
          DEFAULT: "var(--popover)",
          foreground: "var(--popover-foreground)",
        },
        
        primary: {
          DEFAULT: "var(--primary)",
          foreground: "var(--primary-foreground)",
        },
        
        secondary: {
          DEFAULT: "var(--secondary)",
          foreground: "var(--secondary-foreground)",
        },
        
        muted: {
          DEFAULT: "var(--muted)",
          foreground: "var(--muted-foreground)",
        },
        
        accent: {
          DEFAULT: "var(--accent)",
          foreground: "var(--accent-foreground)",
        },
        
        destructive: {
          DEFAULT: "var(--destructive)",
          foreground: "var(--destructive-foreground)",
        },
        
        success: {
          DEFAULT: "var(--success)",
          foreground: "var(--success-foreground)",
        },
        
        warning: {
          DEFAULT: "var(--warning)",
          foreground: "var(--warning-foreground)",
        },
        
        info: {
          DEFAULT: "var(--info)",
          foreground: "var(--info-foreground)",
        },
        
        border: "var(--border)",
        input: "var(--input)",
        ring: "var(--ring)",
        
        // Sidebar
        sidebar: {
          DEFAULT: "var(--sidebar)",
          foreground: "var(--sidebar-foreground)",
          primary: "var(--sidebar-primary)",
          "primary-foreground": "var(--sidebar-primary-foreground)",
          accent: "var(--sidebar-accent)",
          "accent-foreground": "var(--sidebar-accent-foreground)",
          border: "var(--sidebar-border)",
        },
        
        // Status badges
        "status-available": {
          DEFAULT: "var(--status-available)",
          foreground: "var(--status-available-foreground)",
        },
        "status-prepared": {
          DEFAULT: "var(--status-prepared)",
          foreground: "var(--status-prepared-foreground)",
        },
        "status-ready": {
          DEFAULT: "var(--status-ready)",
          foreground: "var(--status-ready-foreground)",
        },
        "status-processed": {
          DEFAULT: "var(--status-processed)",
          foreground: "var(--status-processed-foreground)",
        },
        "status-returned": {
          DEFAULT: "var(--status-returned)",
          foreground: "var(--status-returned-foreground)",
        },
        "status-waived": {
          DEFAULT: "var(--status-waived)",
          foreground: "var(--status-waived-foreground)",
        },
        "status-outstanding": {
          DEFAULT: "var(--status-outstanding)",
          foreground: "var(--status-outstanding-foreground)",
        },
        "status-deferred": {
          DEFAULT: "var(--status-deferred)",
          foreground: "var(--status-deferred-foreground)",
        },
        
        // Neutral scale (for utilities that need specific shades)
        neutral: {
          50: "var(--color-neutral-50)",
          100: "var(--color-neutral-100)",
          200: "var(--color-neutral-200)",
          300: "var(--color-neutral-300)",
          400: "var(--color-neutral-400)",
          500: "var(--color-neutral-500)",
          600: "var(--color-neutral-600)",
          700: "var(--color-neutral-700)",
          800: "var(--color-neutral-800)",
          900: "var(--color-neutral-900)",
          950: "var(--color-neutral-950)",
        },
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
