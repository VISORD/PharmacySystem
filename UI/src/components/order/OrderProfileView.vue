<script setup>
import OrderGeneralInfoView from '@/components/order/OrderGeneralInfoView.vue'
import OrderMedicamentsView from '@/components/order/OrderMedicamentsView.vue'
import OrderHistoryView from '@/components/order/OrderHistoryView.vue'
import { useOrderStore } from '@/stores/order'
import { ref } from 'vue'
import router from '@/plugins/router'

const order = useOrderStore()

const tab = ref(0)

async function show() {
    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, orderId: order.view.orderId }
    })

    tab.value = 0
}

async function hide() {
    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, orderId: undefined }
    })
}
</script>

<template>
    <Dialog
        v-model:visible="order.view.dialog"
        modal
        position="top"
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        :header="order.view.profile.id ? `Order info: Order #${order.view.profile.id}` : 'Order info'"
        class="profile-dialog"
    >
        <TabView class="profile-view-tab" @tab-change="(event) => (tab = event.index)">
            <TabPanel header="General Info">
                <OrderGeneralInfoView v-if="tab === 0" />
            </TabPanel>
            <TabPanel header="Medicaments">
                <OrderMedicamentsView v-if="tab === 1" />
            </TabPanel>
            <TabPanel header="History">
                <OrderHistoryView v-if="tab === 2" />
            </TabPanel>
        </TabView>
    </Dialog>
</template>
