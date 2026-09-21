/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      colors: {
        ink: '#17261f',
        forest: '#1f5c4a',
        mint: '#dff1e8',
        canvas: '#f5f7f3',
        line: '#dce5df',
        coral: '#e27658'
      },
      fontFamily: {
        sans: ['Manrope', 'ui-sans-serif', 'system-ui', 'sans-serif'],
        display: ['DM Sans', 'ui-sans-serif', 'system-ui', 'sans-serif']
      },
      boxShadow: {
        soft: '0 18px 50px rgba(23, 38, 31, 0.08)'
      }
    }
  },
  plugins: []
};
