import { useField } from 'vee-validate'

export const companyNameRule = ({ field = undefined, model = undefined } = {}) =>
    useField(
        field ?? 'name',
        (value) => (!value ? 'Company Name is required' : true),
        model
            ? {
                  modelPropName: model
              }
            : null
    )

export const emailRule = ({ field = undefined, model = undefined } = {}) =>
    useField(
        field ?? 'email',
        (value) => {
            if (!value) {
                return 'Email is required'
            } else if (
                !value.match(
                    /^(([^<>()[\].,;:\s@"]+(\.[^<>()[\].,;:\s@"]+)*)|(".+"))@(([^<>()[\].,;:\s@"]+\.)+[^<>()[\].,;:\s@"]{2,})$/i
                )
            ) {
                return 'Input is not Email'
            }

            return true
        },
        model
            ? {
                  modelPropName: model
              }
            : null
    )

export const phoneRule = ({ field = undefined, model = undefined } = {}) =>
    useField(
        field ?? 'phone',
        (value) => {
            if (value) {
                if (!value.match(/^((8|\+7)[- ]?)?(\(?\d{3}\)?[- ]?)?[\d\- ]{7,10}$/i)) {
                    return 'Input is not Phone'
                }
            }

            return true
        },
        model
            ? {
                  modelPropName: model
              }
            : null
    )

export const passwordRule = (field = undefined) =>
    useField(field ?? 'password', (value) => {
        if (!value) {
            return 'Password is required'
        } else if (value.length < 4) {
            return 'Password length should be greater or equal 4'
        }

        return true
    })
