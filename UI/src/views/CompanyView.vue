<script setup>
import CompanyInfoEditForm from '@/components/company/CompanyInfoEditForm.vue'
import { useCompanyStore } from '@/stores/company'
import { onMounted } from 'vue'

const company = useCompanyStore()
onMounted(async () => await company.reload())
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
                    v-if="!company.loading"
                    style="display: flex; align-items: center; height: 4rem; font-size: 2rem; font-weight: 700"
                >
                    {{ company.data.name }}
                </div>
                <Skeleton v-else width="40rem" height="3rem" style="margin-bottom: 0.5rem; margin-top: 0.5rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-at']" />
            </div>

            <Transition name="company" mode="out-in">
                <div v-if="!company.loading" style="display: flex; align-items: center">{{ company.data.email }}</div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-phone']" />
            </div>

            <Transition name="company" mode="out-in">
                <div v-if="!company.loading" style="display: flex; align-items: center">
                    {{ company.data.phone ?? '—' }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>
        </div>

        <div style="display: flex; align-items: center; justify-content: center; width: 6rem">
            <Button icon="fa-solid fa-pencil" @click="company.dialog = true" :disabled="company.loading" />
            <CompanyInfoEditForm />
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
