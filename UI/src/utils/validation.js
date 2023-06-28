import { useField } from 'vee-validate'

export const companyNameRule = (field = undefined) =>
    useField(field ?? 'name', (value) => {
        if (!value) {
            return 'Company Name is required'
        } else if (value.length > 50) {
            return 'Company Name length should be less or equal 50'
        }

        return true
    })

export const emailRule = (field = undefined) =>
    useField(field ?? 'email', (value) => {
        if (!value) {
            return 'Email is required'
        } else if (
            !value.match(
                /^(([^<>()[\].,;:\s@"]+(\.[^<>()[\].,;:\s@"]+)*)|(".+"))@(([^<>()[\].,;:\s@"]+\.)+[^<>()[\].,;:\s@"]{2,})$/i
            )
        ) {
            return 'Input is not Email'
        } else if (value.length > 255) {
            return 'Email length should be less or equal 255'
        }

        return true
    })

export const phoneRule = (field = undefined) =>
    useField(field ?? 'phone', (value) => {
        if (value) {
            if (!value.match(/^((8|\+7)[- ]?)?(\(?\d{3}\)?[- ]?)?[\d\- ]{7,10}$/i)) {
                return 'Input is not Phone'
            } else if (value.length > 32) {
                return 'Phone length should be less or equal 32'
            }
        }

        return true
    })

export const passwordRule = (field = undefined) =>
    useField(field ?? 'password', (value) => {
        if (!value) {
            return 'Password is required'
        } else if (value.length < 4) {
            return 'Password length should be greater or equal 4'
        } else if (value.length > 255) {
            return 'Password length should be less or equal 255'
        }

        return true
    })

export const descriptionRule = (field = undefined) =>
    useField(field ?? 'description', (value) => {
        if (value?.length > 1024) {
            return 'Description length should be less or equal 1024'
        }

        return true
    })

export const medicamentNameRule = (field = undefined) =>
    useField(field ?? 'name', (value) => {
        if (!value) {
            return 'Medicament Name is required'
        } else if (value.length > 100) {
            return 'Medicament Name length should be less or equal 100'
        }

        return true
    })

export const medicamentVendorPriceRule = (field = undefined) =>
    useField(field ?? 'vendorPrice', (value) => {
        if (!value) {
            return 'Medicament Vendor Price is required'
        } else if (value < 0) {
            return 'Medicament Vendor Price should be non-negative value'
        } else if (value > 1_000_000_000) {
            return 'Medicament Vendor Price should less or equal 1 000 000 000'
        }

        return true
    })
