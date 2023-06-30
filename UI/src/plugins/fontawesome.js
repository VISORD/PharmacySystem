import { dom, library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import {
    faArrowRightFromBracket,
    faArrowsRotate,
    faArrowUpRightFromSquare,
    faAt,
    faCalculator,
    faCalendarDay,
    faCalendarPlus,
    faCheck,
    faCircleCheck,
    faEraser,
    faHandHoldingMedical,
    faLink,
    faLinkSlash,
    faListCheck,
    faLocationDot,
    faLock,
    faMagnifyingGlass,
    faMapLocationDot,
    faMoneyBillWave,
    faPencil,
    faPhone,
    faPlay,
    faPlus,
    faQuoteRight,
    faSpinner,
    faTablets,
    faTrashCan,
    faTriangleExclamation,
    faTruckArrowRight,
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
    faCalendarPlus,
    faCalendarDay,
    faSpinner,
    faCalculator,
    faArrowUpRightFromSquare,
    faLocationDot,
    faLink,
    faLinkSlash,
    faPlay,
    faTruckArrowRight,
    faCircleCheck
)

// <i> => <svg>
dom.watch()

export default function (app) {
    app.component('fa', FontAwesomeIcon)
}
