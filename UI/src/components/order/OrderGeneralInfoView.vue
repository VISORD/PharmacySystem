<script setup>
import { useConfirm } from 'primevue/useconfirm'
import { useOrderStore } from '@/stores/order'
import { resolveOrderStatus } from '@/constants/order-statuses'
import { onMounted } from 'vue'
import router from '@/plugins/router'

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

function toPharmacy() {
    window.open(
        router.resolve({
            path: 'pharmacy',
            query: { pharmacyId: order.view.profile.pharmacy.id }
        }).href,
        '_blank'
    )
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
                <fa :icon="['fas', 'fa-spinner']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" class="profile-view-item">
                    {{ resolveOrderStatus(order.view.profile.status) }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-calendar-plus']" />
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
                <Transition name="profile" mode="out-in">
                    <div class="profile-view-button" v-if="order.view.profile.status === 0">
                        <Button
                            icon="fa-solid fa-play"
                            @click="order.view.tryLaunch()"
                            v-tooltip.left.hover="'Start the execution'"
                            :loading="!order.view.buttons"
                        />
                    </div>

                    <div class="profile-view-button" v-else-if="order.view.profile.status === 1">
                        <Button
                            icon="fa-solid fa-truck-arrow-right"
                            @click="order.view.tryShip()"
                            v-tooltip.left.hover="'Ship medicaments'"
                            :loading="!order.view.buttons"
                        />
                    </div>

                    <div class="profile-view-button" v-else-if="order.view.profile.status === 2">
                        <Button
                            icon="fa-solid fa-circle-check"
                            @click="order.view.tryComplete()"
                            v-tooltip.left.hover="'Complete the order'"
                            :loading="!order.view.buttons"
                        />
                    </div>
                </Transition>

                <Transition name="profile" mode="out-in">
                    <div class="profile-view-button" v-if="order.view.profile.status === 0">
                        <Button
                            icon="fa-solid fa-trash-can"
                            severity="danger"
                            @click="confirmDelete()"
                            v-tooltip.left.hover="'Delete the order'"
                            :loading="!order.view.buttons"
                        />
                    </div>
                </Transition>
            </div>
        </div>
    </div>

    <Divider />

    <div style="display: flex; justify-content: space-between">
        <div class="profile-view">
            <div class="profile-view-header-icon">
                <Avatar icon="fa-solid fa-hand-holding-medical" size="large" class="profile-view-header-icon-avatar" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" class="profile-view-header">
                    {{ order.view.profile.pharmacy.name }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-header-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-map-location-dot']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!order.view.loading" class="profile-view-item">
                    {{ order.view.profile.pharmacy.address }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>
        </div>

        <div style="display: flex; justify-content: center; width: 6rem">
            <div class="profile-view-button" v-tooltip.left.hover="'View in new window'">
                <Button
                    icon="fa-solid fa-arrow-up-right-from-square"
                    severity="info"
                    text
                    @click="toPharmacy()"
                    :disabled="order.view.loading"
                />
            </div>
        </div>
    </div>
</template>

<style scoped></style>
