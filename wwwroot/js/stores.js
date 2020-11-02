
const iconBase =
    "https://maps.google.com/mapfiles/kml/pushpin/";
const icons = {
    regularStore: {
        icon: iconBase + "red-pushpin.png",
    },
    warehouseStore: {
        icon: iconBase + "ylw-pushpin.png",
    },
};

var storesObject = [];


function initMap() {
    const map = new google.maps.Map(document.getElementById("map"),
        {
            zoom: 15,
            center: { lat: 32.086813, lng: 34.77630 }
        });
    geocodeStores(map, storesObject);
};


function geocodeStores(map, stores) {
    const geocoder = new google.maps.Geocoder();

    for (let i = 0; i < stores; i++) {
        let address = stores[i][0];
        let name = stores[i][1];
        let isWarehouse = stores[i][2];
        geocoder.geocode({ address: address },
            (results, status) => {
                if (status === "OK") {
                    new google.maps.Marker({
                            position: results[0].geometry.location,
                            map,
                            title: name,
                            icon: {
                                url: isWarehouse ? icons[warehouseStore].icon : icons[regularStore].icon,
                                scaledSize: new google.maps.Size(50, 50)
                            }
                        }
                    );
                    console.log("ok");

                } else {
                    alert(`Geocode was not successful for the following reason: ${status}`);
                }
            });
    }

}