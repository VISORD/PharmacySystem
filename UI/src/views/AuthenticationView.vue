<script setup>
import { ref } from 'vue'
import SignInForm from '@/components/SignInForm.vue'
import SignUpForm from '@/components/SignUpForm.vue'

const value = ref(1)
const options = ref([
    { name: 'Sign In', value: 1 },
    { name: 'Sign Up', value: 2 }
])
</script>

<template>
    <div class="non-authenticated-page">
        <div class="page authentication">
            <header>
                <div>
                    <h1>Pharmacy System</h1>
                </div>
                <div style="margin-bottom: 60px">
                    <span>Make pharmacies great again!</span>
                </div>
                <SelectButton
                    v-model="value"
                    :options="options"
                    aria-labelledby="basic"
                    optionLabel="name"
                    optionValue="value"
                    unselectable
                />
            </header>
            <div class="separator">
                <div />
            </div>
            <main>
                <Transition :name="value === 1 ? 'sign-in' : 'sign-out'" mode="out-in">
                    <SignInForm v-if="value === 1" />
                    <SignUpForm v-else />
                </Transition>
            </main>
        </div>
    </div>
</template>

<style scoped>
.non-authenticated-page {
    min-height: 100vh;
    display: flex;
    place-items: center;
    justify-content: center;
}

.authentication {
    display: grid;
    grid-template-columns: 1fr 0.25fr 1fr;
    min-height: 400px;
}

.authentication > header {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
}

.authentication > .separator {
    display: flex;
    align-items: center;
    justify-content: center;
}

.authentication > .separator > div {
    width: 1px;
    height: 100%;
    border: 1px solid var(--primary-color);
}

.authentication > main {
    display: flex;
    align-items: center;
    justify-content: center;
}

.sign-in-enter-active {
    transition: all 0.3s ease-out;
}

.sign-in-enter-from {
    transform: translateX(20px);
    opacity: 0;
}

.sign-out-enter-active {
    transition: all 0.3s ease-in;
}

.sign-out-enter-from {
    transform: translateX(-20px);
    opacity: 0;
}
</style>
