import { dom, library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import {
    faArrowRightFromBracket,
    faAt,
    faCheck,
    faHandHoldingMedical,
    faListCheck,
    faLock,
    faTablets,
    faTriangleExclamation,
    faUnlock,
    faUsersBetweenLines,
    faXmark
} from '@fortawesome/free-solid-svg-icons'

library.add(
    faTablets,
    faHandHoldingMedical,
    faListCheck,
    faUsersBetweenLines,
    faArrowRightFromBracket,
    faTriangleExclamation,
    faAt,
    faLock,
    faUnlock,
    faXmark,
    faCheck
)

// <i> => <svg>
dom.watch()

export default function (app) {
    app.component('fa', FontAwesomeIcon)
}
