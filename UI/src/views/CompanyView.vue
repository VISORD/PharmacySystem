<script setup>
import CompanyInfoForm from '@/components/company/CompanyInfoForm.vue'
import { useCompanyStore } from '@/stores/company'
import { onMounted } from 'vue'

const company = useCompanyStore()

onMounted(async () => await company.reload())
</script>

<template>
    <CompanyInfoForm />

    <div style="display: flex; justify-content: space-between">
        <div class="profile-view">
            <div class="profile-view-header-icon">
                <Avatar icon="fa-solid fa-users-between-lines" size="large" class="profile-view-header-icon-avatar" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!company.loading" class="profile-view-header">
                    {{ company.data.name }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-header-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-at']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!company.loading" class="profile-view-item">
                    {{ company.data.email }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-phone']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!company.loading" class="profile-view-item">
                    {{ company.data.phone ?? '—' }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>
        </div>

        <div style="display: flex; justify-content: center; width: 6rem">
            <div style="display: flex; flex-direction: column">
                <div
                    style="
                        display: flex;
                        flex-direction: column;
                        align-items: center;
                        justify-content: center;
                        height: 4rem;
                    "
                    v-tooltip.left.hover="'Edit the company info'"
                >
                    <Button icon="fa-solid fa-pencil" @click="company.dialog = true" :disabled="company.loading" />
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped></style>
