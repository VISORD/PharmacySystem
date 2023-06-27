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
            <div style="display: flex; align-items: center; justify-content: center">
                <Avatar
                    icon="fa-solid fa-list-check"
                    size="large"
                    style="background-color: var(--text-color); color: var(--primary-color-text)"
                />
            </div>

            <Transition name="profile" mode="out-in">
                <div
                    v-if="!order.view.loading"
                    style="display: flex; align-items: center; height: 4rem; font-size: 2rem; font-weight: 700"
                >
                    Order #{{ order.view.profile.id }}
                </div>
                <Skeleton v-else width="40rem" height="3rem" style="margin-bottom: 0.5rem; margin-top: 0.5rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-hand-holding-medical']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" style="display: flex; align-items: center">
                    {{ order.view.profile.pharmacy.name }} ({{ order.view.profile.pharmacy.address }})
                </div>
                <Skeleton v-else width="40rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-spinner']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" style="display: flex; align-items: center">
                    {{ resolveOrderStatus(order.view.profile.status) }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-calendar-check']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" style="display: flex; align-items: center">
                    {{ order.view.profile.orderedAtText ?? '—' }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-calendar-day']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" style="display: flex; align-items: center">
                    {{ order.view.profile.updatedAtText }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
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
