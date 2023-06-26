import YmapPlugin from 'vue-yandex-maps'

export default function (app) {
    app.use(YmapPlugin, {
        apiKey: import.meta.env.VITE_YAMAPS_API_KEY,
        lang: 'en_US',
        coordorder: 'latlong',
        enterprise: false,
        version: '2.1'
    })
}
