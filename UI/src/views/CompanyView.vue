<script setup>
import { onMounted, ref } from 'vue'
import { get } from '@/api/company'
import { useToast } from 'primevue/usetoast'
import CompanyInfoEditForm from '@/components/company/CompanyInfoEditForm.vue'

const loading = ref(true)
const companyEditDialog = ref(null)

const data = ref({})

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
}

onMounted(async () => await reload())
</script>

<template>
    <div style="display: flex; justify-content: space-between">
        <div class="company" style="flex: 1">
            <div style="display: flex; align-items: center; justify-content: center">
                <Avatar
                    icon="fa-solid fa-users-between-lines"
                    size="large"
                    style="background-color: var(--text-color); color: var(--primary-color-text)"
                />
            </div>

            <Transition name="company" mode="out-in">
                <div
                    v-if="!loading"
                    style="display: flex; align-items: center; height: 4rem; font-size: 2rem; font-weight: 700"
                >
                    {{ data.name }}
                </div>
                <Skeleton v-else width="40rem" height="3rem" style="margin-bottom: 0.5rem; margin-top: 0.5rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-at']" />
            </div>

            <Transition name="company" mode="out-in">
                <div v-if="!loading" style="display: flex; align-items: center">{{ data.email }}</div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-phone']" />
            </div>

            <Transition name="company" mode="out-in">
                <div v-if="!loading" style="display: flex; align-items: center">{{ data.phone ?? '—' }}</div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>
        </div>

        <div style="display: flex; align-items: center; justify-content: center; width: 6rem">
            <Button icon="fa-solid fa-pencil" @click="companyEditDialog.active = true" :disabled="loading" />
            <CompanyInfoEditForm
                ref="companyEditDialog"
                :name="data.name"
                :email="data.email"
                :phone="data.phone"
                @apply="reload()"
            />
        </div>
    </div>
</template>

<style scoped>
.company-enter-active {
    transition: opacity 0.3s ease;
}

.company-enter-from {
    opacity: 0;
}

.company {
    display: grid;
    grid-template-columns: 6rem fit-content(100%);
}
</style>
