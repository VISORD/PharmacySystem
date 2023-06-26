import { dom, library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import {
    faArrowRightFromBracket,
    faArrowsRotate,
    faAt,
    faCalendarCheck,
    faCalendarDay,
    faCheck,
    faEraser,
    faHandHoldingMedical,
    faListCheck,
    faLock,
    faMagnifyingGlass,
    faMapLocationDot,
    faMoneyBillWave,
    faPencil,
    faPhone,
    faPlus,
    faQuoteRight,
    faSpinner,
    faTablets,
    faTrashCan,
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
    faCheck,
    faArrowsRotate,
    faPlus,
    faEraser,
    faPhone,
    faPencil,
    faTrashCan,
    faMagnifyingGlass,
    faMapLocationDot,
    faMoneyBillWave,
    faQuoteRight,
    faCalendarCheck,
    faCalendarDay,
    faSpinner
)

// <i> => <svg>
dom.watch()

export default function (app) {
    app.component('fa', FontAwesomeIcon)
}
