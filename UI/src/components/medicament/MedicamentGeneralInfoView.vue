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
            <div class="profile-view-header-icon">
                <Avatar icon="fa-solid fa-tablets" size="large" class="profile-view-header-icon-avatar" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!medicament.view.loading" class="profile-view-header">
                    {{ medicament.view.profile.name }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-header-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-money-bill-wave']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!medicament.view.loading" class="profile-view-item">
                    {{ medicament.view.profile.vendorPriceText }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-quote-right']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!medicament.view.loading" class="profile-view-item">
                    {{ medicament.view.profile.description ?? '—' }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-item-skeleton" />
            </Transition>
        </div>

        <div style="display: flex; justify-content: center; width: 6rem">
            <div style="display: flex; flex-direction: column">
                <div class="profile-view-button">
                    <Button
                        icon="fa-solid fa-pencil"
                        @click="medicament.edit.dialog = true"
                        :disabled="medicament.view.loading"
                        v-tooltip.left.hover="'Edit the medicament'"
                    />
                </div>

                <div class="profile-view-button">
                    <Button
                        icon="fa-solid fa-trash-can"
                        severity="danger"
                        @click="confirmDelete()"
                        :disabled="medicament.view.loading"
                        v-tooltip.left.hover="'Delete the medicament'"
                    />
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped></style>
