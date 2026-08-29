/**
 * Tailwind CSS 3 (reines JavaScript, laeuft auch auf 32-Bit/ia32).
 *
 * Tailwind 4 wurde hier ueber einen `@theme`-Block in src/index.css
 * konfiguriert; dessen Custom Properties erzeugten die Utilities automatisch.
 * In v3 gibt es das nicht, deshalb stehen dieselben Werte hier -- und
 * zusaetzlich als CSS-Variablen in :root, weil index.css sie direkt per
 * var(--color-...) verwendet.
 */

/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      colors: {
        obsidian: '#0a0c10',
        card: '#12161c',
        border: '#232a34',
        text: {
          DEFAULT: '#e6e3da',
          muted: '#8b93a1',
        },
        turquoise: '#3fc7c1',
        gold: {
          DEFAULT: '#d9a441',
          light: '#f0c674',
        },
        ember: {
          DEFAULT: '#c1432b',
          light: '#e8683f',
        },
      },
      // In v4 ist die Rahmenfarbe von `border` currentColor, in v3 sonst
      // gray-200 -- das waere im dunklen Layout ein greller Ausreisser, falls
      // irgendwo `border` ohne explizite Farbe steht.
      borderColor: {
        DEFAULT: '#232a34',
      },
      fontFamily: {
        eyebrow: ['Cinzel', 'serif'],
        headline: ['Barlow Condensed', 'sans-serif'],
        body: ['Inter', 'sans-serif'],
        mono: ['JetBrains Mono', 'monospace'],
      },
    },
  },
  plugins: [],
};
