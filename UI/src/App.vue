<script setup>
import { RouterView } from 'vue-router'
</script>

<template>
    <Toast />
    <RouterView v-slot="{ Component }">
        <Transition name="app" mode="out-in">
            <Component :is="Component" />
        </Transition>
    </RouterView>
</template>

<style scoped>
.app-enter-active,
.app-leave-active {
    transition: opacity 0.3s ease;
}

.app-enter-from,
.app-leave-to {
    opacity: 0;
}
</style>
