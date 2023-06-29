const searching = 'Searching...'

function createMarker(coords) {
    return new ymaps.Placemark(
        coords,
        { iconCaption: searching },
        {
            draggable: true,
            preset: 'islands#violetDotIcon'
        }
    )
}

async function addMarker(map, form, coords) {
    if (map.marker) {
        map.marker.geometry.setCoordinates(coords)
    } else {
        // eslint-disable-next-line no-undef
        map.marker = createMarker(coords)
        map.instance.geoObjects.add(map.marker)

        map.marker.events.add('dragend', async function () {
            await getAddress(map, form, map.marker.geometry.getCoordinates())
        })
    }

    await getAddress(map, form, coords)
}

async function getAddress(map, form, coords) {
    map.marker.properties.set('iconCaption', searching)

    const res = await ymaps.geocode(coords, { results: 1 })
    const firstGeoObject = res.geoObjects.get(0)

    const iconCaption = [
        firstGeoObject.getLocalities().length
            ? firstGeoObject.getLocalities()
            : firstGeoObject.getAdministrativeAreas(),
        firstGeoObject.getThoroughfare() || firstGeoObject.getPremise()
    ]
        .filter(Boolean)
        .join(', ')

    map.marker.properties.set({
        iconCaption: iconCaption,
        balloonContent: firstGeoObject.getAddressLine()
    })

    map.latitude = coords[0]
    map.longitude = coords[1]
    map.precision = firstGeoObject?.properties.get('metaDataProperty.GeocoderMetaData.precision')
    form.value.address.setValue(map.marker?.properties.get('balloonContent'))
}

export async function findCoordsByAddress(map, form, address) {
    const res = await ymaps.geocode(address, { results: 1 })
    const firstGeoObject = res.geoObjects.get(0)
    if (firstGeoObject) {
        map.latitude = firstGeoObject.geometry.getCoordinates()[0]
        map.longitude = firstGeoObject.geometry.getCoordinates()[1]
    }

    map.precision = firstGeoObject?.properties.get('metaDataProperty.GeocoderMetaData.precision')
    form.value.address.setValue(address)

    await addMarker(map, form, firstGeoObject.geometry.getCoordinates())
}

export async function watch(map, form, coords) {
    if (coords.latitude && coords.longitude) {
        await addMarker(map, form, [coords.latitude, coords.longitude])
    }

    map.instance.events.add('click', async function (event) {
        await addMarker(map, form, event.get('coords'))
    })

    map.suggestions = new ymaps.SuggestView('address-prompt')
    map.suggestions.events.add('select', async function (event) {
        await findCoordsByAddress(map, form, event.get('item').value)
    })
}
