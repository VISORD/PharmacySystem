<script setup>
import OrderGeneralInfoView from '@/components/order/OrderGeneralInfoView.vue'
import OrderMedicamentsView from '@/components/order/OrderMedicamentsView.vue'
import OrderHistoryView from '@/components/order/OrderHistoryView.vue'
import { useOrderStore } from '@/stores/order'

const order = useOrderStore()
</script>

<template>
    <Dialog
        v-model:visible="order.view.dialog"
        modal
        dismissable-mask
        :header="order.view.profile ? `Order info: Order #${order.view.profile.id}` : 'Order info'"
        style="width: 85vw; margin: 5rem"
    >
        <TabView>
            <TabPanel header="General Info">
                <OrderGeneralInfoView />
            </TabPanel>
            <TabPanel header="Medicaments">
                <OrderMedicamentsView />
            </TabPanel>
            <TabPanel header="History">
                <OrderHistoryView />
            </TabPanel>
        </TabView>
    </Dialog>
</template>
