<script setup>
import { ref } from 'vue'
import { useConfirm } from 'primevue/useconfirm'
import { useAccountStore } from '@/stores/account'

const confirm = useConfirm()
const account = useAccountStore()

const menu = ref([
    { label: 'Company', icon: 'fa-solid fa-users-between-lines', to: '/' },
    { label: 'Pharmacies', icon: 'fa-solid fa-hand-holding-medical', to: '/pharmacy' },
    { label: 'Medicaments', icon: 'fa-solid fa-tablets', to: '/medicament' },
    { label: 'Orders', icon: 'fa-solid fa-list-check', to: '/order' }
])

const confirmSignOut = () => {
    confirm.require({
        message: 'Are you sure you want to sign out?',
        header: 'Confirmation',
        icon: 'fa-solid fa-triangle-exclamation',
        acceptIcon: 'fa-solid fa-check',
        rejectIcon: 'fa-solid fa-xmark',
        accept: async () => await account.trySignOut(),
        reject: () => {}
    })
}
</script>

<template>
    <Menubar class="main-menu" :model="menu">
        <template #end>
            <ConfirmDialog />
            <Button
                class="sign-out"
                @click="confirmSignOut()"
                icon="fa-solid fa-arrow-right-from-bracket"
                label="Sign Out"
                text
            />
        </template>
    </Menubar>
</template>

<style scoped>
.main-menu {
    font-weight: bold;
    margin-bottom: 2em;
}

.sign-out {
    color: var(--text-color) !important;
}
</style>
