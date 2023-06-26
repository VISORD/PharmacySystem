import '@/styles/main.css'
import '@/styles/fonts.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from '@/App.vue'
import router from '@/plugins/router'
import fontawesome from '@/plugins/fontawesome'
import primevue from '@/plugins/primevue'
import yamaps from '@/plugins/yamaps'

const app = createApp(App)

app.use(createPinia())
app.use(router)
fontawesome(app)
primevue(app)
yamaps(app)

app.mount('#app')
