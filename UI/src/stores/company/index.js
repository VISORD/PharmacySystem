import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { get, update } from '@/api/company'

export const useCompanyStore = defineStore('company', () => {
    const loading = ref(true)
    const dialog = ref(false)
    const data = ref({})

    const processing = ref(false)

    const toast = useToast()

    async function reload() {
        loading.value = true
        processing.value = true

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

        processing.value = false
        loading.value = false
    }

    async function tryUpdate(values) {
        processing.value = true

        const response = await update(values)

        let notification
        if (response.status < 400) {
            dialog.value = false
            await reload()

            notification = {
                severity: 'success',
                summary: 'Company info has updated',
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

        processing.value = false
    }

    return { dialog, loading, processing, data, reload, tryUpdate }
})
