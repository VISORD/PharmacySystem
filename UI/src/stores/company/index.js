import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { get, update } from '@/api/company'

export const useCompanyStore = defineStore('company', () => {
    const loading = ref(true)
    const dialog = ref(false)
    const data = ref({})
    const before = ref(() => {})

    const toast = useToast()

    async function reload() {
        loading.value = true

        const response = await get()
        if (response.status < 400) {
            data.value = response.data.item
        } else if (response.status !== 401) {
            toast.add({
                severity: 'error',
                summary: 'Company info getting failed',
                detail: response.data.error,
                life: 3000
            })
        }

        loading.value = false

        before.value(data.value)
    }

    async function tryUpdate(values) {
        const response = await update(values)

        let notification
        if (response.status < 400) {
            dialog.value = false
            await reload()

            notification = {
                severity: 'success',
                summary: 'Company info updated',
                detail: 'The operation has been successfully performed',
                life: 3000
            }
        } else if (response.status !== 401) {
            notification = {
                severity: 'error',
                summary: 'Company info update failed',
                detail: response.data.error,
                life: 3000
            }
        }

        if (notification) {
            toast.add(notification)
        }
    }

    return { dialog, loading, data, before, reload, tryUpdate }
})
