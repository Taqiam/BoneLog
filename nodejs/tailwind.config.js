/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "../../**/*.razor",
        "../index.html"
    ],
    theme: {
        extend: {
            fontFamily: {
                serif: ['Vazirmatn', 'ui-sans-serif', 'system-ui'],
            },
},
    },
    plugins: [
        require('@tailwindcss/typography'),
    ],
}
