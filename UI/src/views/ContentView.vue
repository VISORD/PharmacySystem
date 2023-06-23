<script setup>
import MainMenu from '@/components/MainMenu.vue'
import { RouterView } from 'vue-router'
</script>

<template>
    <div class="authenticated-page">
        <div class="page">
            <header>
                <MainMenu />
            </header>
            <main>
                <RouterView />
            </main>
        </div>
    </div>
</template>

<style scoped>
.authenticated-page {
    padding: 20px;
}
</style>
