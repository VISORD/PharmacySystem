export function formatTime(date) {
    const hours = date.getHours()
    const minutes = date.getMinutes()
    const seconds = date.getSeconds()

    return `${hours < 10 ? '0' : ''}${hours}:${minutes < 10 ? '0' : ''}${minutes}:${seconds < 10 ? '0' : ''}${seconds}`
}

export function formatDate(date) {
    const day = date.getDate()
    const month = date.getMonth() + 1
    const year = date.getFullYear()

    return `${year}-${month < 10 ? '0' : ''}${month}-${day < 10 ? '0' : ''}${day}`
}

export function parseTime(value) {
    const hours = value.substring(0, 2)
    const minutes = value.substring(3, 5)
    const seconds = value.substring(6, 8)

    const date = new Date()
    date.setHours(parseInt(hours, 10), parseInt(minutes, 10), parseInt(seconds, 10))

    return date
}
