
const iconBase =
    "https://maps.google.com/mapfiles/kml/pushpin/";
const icons = {
    red_pushpin: {
        icon: iconBase + "red-pushpin.png",
    },
    ylw_pushpin: {
        icon: iconBase + "ylw-pushpin.png",
    },
};

const stores = [
    ["GamingStore Warehouse", 32.086813, 34.776303, 1, "ylw_pushpin"],
    ["Store 1", 32.084813, 34.777303, "red_pushpin"],
    ["Store 2", 32.082813, 34.778303, "red_pushpin"],
    ["Store 3", 32.080813, 34.779303, "red_pushpin"],
];

function initMap() {
    const map = new google.maps.Map(document.getElementById("map"),
        {
            zoom: 15,
            center: { lat: 32.086813, lng: 34.77630 }
        });
    setMarkers(map);
};


function setMarkers(map) {

    for (let storeIndex = 0; storeIndex < stores.length; storeIndex++) {
        const store = stores[storeIndex];
        new google.maps.Marker({
            position: { lat: store[1], lng: store[2] },
            map,
            title: store[0],
            icon: {
                url: icons[store[3]].icon,
                scaledSize: new google.maps.Size(50, 50)
            }
        });
    }
}

function setStores(storesObject) {
    for (let storeIndex = 0; storeIndex < Object.keys(storesObject).length; storeIndex++) {
        var store = Object.keys(storesObject[storeIndex]);
        console.log(Object.keys(store));
    }
    /*for (let store in storesObject) {
            stores.push([store.Name, store.GeoLocation.Latitude, store.GeoLocation.Longitude, "red_pushpin"]);
    }*/
}