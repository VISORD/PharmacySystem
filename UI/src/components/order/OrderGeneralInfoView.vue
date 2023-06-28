<script setup>
import { useConfirm } from 'primevue/useconfirm'
import { useOrderStore } from '@/stores/order'
import { resolveOrderStatus } from '@/constants/order-statuses'
import { onMounted } from 'vue'

const confirm = useConfirm()
const order = useOrderStore()

const confirmDelete = () => {
    confirm.require({
        group: 'order-view-delete',
        header: 'Confirmation',
        icon: 'fa-solid fa-triangle-exclamation',
        acceptIcon: 'fa-solid fa-check',
        rejectIcon: 'fa-solid fa-xmark',
        accept: async () => await order.view.tryDelete(),
        reject: () => {}
    })
}

onMounted(async () => await order.view.reload())
</script>

<template>
    <ConfirmDialog group="order-view-delete">
        <template #message>
            <div>
                Are you sure you want to delete order<b>#{{ order.view.orderId }}</b
                >?
            </div>
        </template>
    </ConfirmDialog>

    <div style="display: flex; justify-content: space-between">
        <div class="profile-view">
            <div class="profile-view-header-icon">
                <Avatar icon="fa-solid fa-list-check" size="large" class="profile-view-header-icon-avatar" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" class="profile-view-header">Order #{{ order.view.profile.id }}</div>
                <Skeleton v-else width="40rem" class="profile-view-header-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-hand-holding-medical']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" class="profile-view-item">
                    {{ order.view.profile.pharmacy.name }} ({{ order.view.profile.pharmacy.address }})
                </div>
                <Skeleton v-else width="40rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-spinner']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" class="profile-view-item">
                    {{ resolveOrderStatus(order.view.profile.status) }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-calendar-check']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" class="profile-view-item">
                    {{ order.view.profile.orderedAtText ?? '—' }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-calendar-day']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" class="profile-view-item">
                    {{ order.view.profile.updatedAtText }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
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
