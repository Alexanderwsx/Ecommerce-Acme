"use strict";

function initMap() {
    const CONFIGURATION = {
        "ctaTitle": "Checkout",
        "mapOptions": { "center": { "lat": 37.4221, "lng": -122.0841 }, "fullscreenControl": true, "mapTypeControl": false, "streetViewControl": true, "zoom": 11, "zoomControl": true, "maxZoom": 22, "mapId": "" },
        "mapsApiKey": "YOUR_API_KEY_HERE",
        "capabilities": { "addressAutocompleteControl": true, "mapDisplayControl": false, "ctaControl": true }
    };
    const componentForm = [
        'OrderHeader_StreetAddress',
        'OrderHeader_City',
        'OrderHeader_State',
        'OrderHeader_Country',
        'OrderHeader_PostalCode',
    ];

    const getFormInputElement = (component) => document.getElementById(component);
    const autocompleteInput = getFormInputElement('OrderHeader_StreetAddress');

    let isAutocompleteInitialized = false; // Variable para rastrear si el Autocomplete ya ha sido inicializado
    let autocomplete;

    autocompleteInput.addEventListener('input', function () {
        if (autocompleteInput.value.length >= 3 && !isAutocompleteInitialized) {
            // Inicializar el Autocomplete solo si no ha sido inicializado anteriormente
            autocomplete = new google.maps.places.Autocomplete(autocompleteInput, {
                fields: ["address_components", "geometry", "name"],
                types: ["address"],
            });

            autocomplete.addListener('place_changed', function () {
                const place = autocomplete.getPlace();
                if (!place.geometry) {
                    window.alert('No details available for input: \'' + place.name + '\'');
                    return;
                }
                fillInAddress(place);
            });

            isAutocompleteInitialized = true; // Marcar que el Autocomplete ha sido inicializado
        }
    });

    function fillInAddress(place) {
        const addressNameFormat = {
            'street_number': 'short_name',
            'route': 'long_name',
            'locality': 'long_name',
            'administrative_area_level_1': 'short_name',
            'country': 'long_name',
            'postal_code': 'short_name',
        };
        const getAddressComp = function (type) {
            for (const component of place.address_components) {
                if (component.types[0] === type) {
                    return component[addressNameFormat[type]];
                }
            }
            return '';
        };
        getFormInputElement('OrderHeader_StreetAddress').value = getAddressComp('street_number') + ' ' + getAddressComp('route');
        getFormInputElement('OrderHeader_City').value = getAddressComp('locality');
        getFormInputElement('OrderHeader_State').value = getAddressComp('administrative_area_level_1');
        getFormInputElement('OrderHeader_PostalCode').value = getAddressComp('postal_code');
        getFormInputElement('OrderHeader_Country').value = getAddressComp('country');
    }
}
