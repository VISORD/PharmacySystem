<script setup>
import { useOrderStore } from '@/stores/order'
import { onMounted } from 'vue'

const order = useOrderStore()

onMounted(async () => await order.view.tryGetHistory())
</script>

<template>
    <Timeline :value="order.view.history" data-key="id">
        <template #opposite="{ item }">
            <div class="p-text-secondary">{{ item.timestampText }}</div>
        </template>
        <template #content="{ item }">
            <p>{{ item.event }}</p>
        </template>
    </Timeline>
</template>

<style scoped></style>
