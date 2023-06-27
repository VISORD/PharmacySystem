<script setup>
import { useMedicamentStore } from '@/stores/medicament'
import { useConfirm } from 'primevue/useconfirm'
import { onMounted } from 'vue'

const confirm = useConfirm()
const medicament = useMedicamentStore()

const confirmDelete = () => {
    confirm.require({
        group: 'medicament-view-delete',
        header: 'Confirmation',
        icon: 'fa-solid fa-triangle-exclamation',
        acceptIcon: 'fa-solid fa-check',
        rejectIcon: 'fa-solid fa-xmark',
        accept: async () => await medicament.view.tryDelete(),
        reject: () => {}
    })
}

onMounted(async () => await medicament.view.reload())
</script>

<template>
    <ConfirmDialog group="medicament-view-delete">
        <template #message>
            <div>
                Are you sure you want to delete '<b>{{ medicament.view.profile.name }}</b
                >' medicament?
            </div>
        </template>
    </ConfirmDialog>

    <div style="display: flex; justify-content: space-between">
        <div class="profile-view">
            <div style="display: flex; align-items: center; justify-content: center">
                <Avatar
                    icon="fa-solid fa-tablets"
                    size="large"
                    style="background-color: var(--text-color); color: var(--primary-color-text)"
                />
            </div>

            <Transition name="profile" mode="out-in">
                <div
                    v-if="!medicament.view.loading"
                    style="display: flex; align-items: center; height: 4rem; font-size: 2rem; font-weight: 700"
                >
                    {{ medicament.view.profile.name }}
                </div>
                <Skeleton v-else width="40rem" height="3rem" style="margin-bottom: 0.5rem; margin-top: 0.5rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-money-bill-wave']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!medicament.view.loading" style="display: flex; align-items: center">
                    {{ medicament.view.profile.vendorPriceText }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-quote-right']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!medicament.view.loading" style="display: flex; align-items: center">
                    {{ medicament.view.profile.description ?? '—' }}
                </div>
                <Skeleton v-else width="40rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>
        </div>

        <div style="display: flex; justify-content: center; width: 6rem">
            <div style="display: flex; flex-direction: column">
                <div class="profile-view-button">
                    <Button icon="fa-solid fa-pencil" />
                </div>
                <div class="profile-view-button">
                    <Button icon="fa-solid fa-trash-can" severity="danger" @click="confirmDelete()" />
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped></style>
