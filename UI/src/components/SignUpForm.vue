<script setup>
import { useField, useForm } from 'vee-validate'
import { useAccountStore } from '@/stores/account'

const { handleSubmit } = useForm()
const account = useAccountStore()

const { value: email, errorMessage: emailErrorMessage } = useField('email', (value) => {
    if (!value) {
        return 'Email is required'
    } else if (
        !value.match(
            /^(([^<>()[\].,;:\s@"]+(\.[^<>()[\].,;:\s@"]+)*)|(".+"))@(([^<>()[\].,;:\s@"]+\.)+[^<>()[\].,;:\s@"]{2,})$/i
        )
    ) {
        return 'Input is not email'
    }
    return true
})

const { value: name, errorMessage: nameErrorMessage } = useField('name', (value) =>
    !value ? 'Name is required' : true
)

const { value: password, errorMessage: passwordErrorMessage } = useField('password', (value) => {
    if (!value) {
        return 'Password is required'
    } else if (value.length < 4) {
        return 'Password length should be greater or equal 4'
    }

    return true
})

const onSubmit = handleSubmit(async (values) => await account.trySignUp(values))
</script>

<template>
    <div class="flex justify-content-center p-fluid">
        <form @submit="onSubmit" class="w-18rem">
            <div class="field">
                <div class="p-input-icon-right">
                    <fa class="field-icon" :icon="['fas', 'at']" />
                    <InputText
                        id="email"
                        v-model="email"
                        type="text"
                        placeholder="Email"
                        :class="{ 'p-invalid': emailErrorMessage }"
                        aria-describedby="text-error"
                        autofocus
                        autocomplete="email"
                    />
                </div>
                <small class="p-error" id="text-error">{{ emailErrorMessage || '&nbsp;' }}</small>
            </div>

            <div class="field">
                <div class="p-input-icon-right">
                    <fa class="field-icon" :icon="['fas', 'users-between-lines']" />
                    <InputText
                        id="name"
                        v-model="name"
                        type="text"
                        placeholder="Company Name"
                        :class="{ 'p-invalid': nameErrorMessage }"
                        aria-describedby="text-error"
                        autocomplete="name"
                    />
                </div>
                <small class="p-error" id="text-error">{{ nameErrorMessage || '&nbsp;' }}</small>
            </div>

            <div class="field">
                <Password
                    id="password"
                    input-id="password-input"
                    v-model="password"
                    placeholder="Password"
                    :class="{ 'p-invalid': passwordErrorMessage }"
                    aria-describedby="text-error"
                    toggle-mask
                >
                    <template #header>
                        <div style="padding-top: 20px" />
                    </template>
                    <template #footer>
                        <Divider />
                        <p class="mt-2">Suggestions</p>
                        <div style="padding-left: 10px">
                            <ul class="pl-2 ml-2 mt-0" style="line-height: 1.5">
                                <li>At least one lowercase</li>
                                <li>At least one uppercase</li>
                                <li>At least one numeric</li>
                                <li>Minimum 8 characters</li>
                            </ul>
                        </div>
                    </template>
                    <template #hideicon="scope">
                        <fa class="field-icon" :icon="['fas', 'unlock']" @click="scope.onClick()" />
                    </template>
                    <template #showicon="scope">
                        <fa class="field-icon" :icon="['fas', 'lock']" @click="scope.onClick()" />
                    </template>
                </Password>
                <small class="p-error" id="text-error">{{ passwordErrorMessage || '&nbsp;' }}</small>
            </div>

            <Button type="submit" label="Submit" class="mt-2" />
        </form>
    </div>
</template>

<style scoped>
.field-icon {
    align-content: center;
    width: 20px;
}
</style>
