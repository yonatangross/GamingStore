
const iconBase =
    "https://maps.google.com/mapfiles/kml/pushpin/";
const icons = {
    regularStore: {
        icon: iconBase + "red-pushpin.png",
    },
    warehouseStore: {
        icon: iconBase + "ylw-pushpin.png",
    }
};

var storeIndex = 0;
var delay = 100;
console.log(`first`);


function initMap() {
    const map = new google.maps.Map(document.getElementById("map"),
        {
            zoom: 15,
            center: { lat: 32.086813, lng: 34.77630 }
        });
    const geocoder = new google.maps.Geocoder();


    /*while (storeIndex < storesObject.length) {
        getStoreAddress(map, geocoder, storesObject[storeIndex], storeIndex);
        storeIndex++;
    };*/
}


function getStoreAddress(map, geocoder, store, storeIndex) {
    let [address, name, isWarehouse] = store;
    console.log(`store: ${name} index:${storeIndex}`);
    geocoder.geocode({ address: address },
        (results, status) => {
            if (status === "OK") {
                console.log(`store ${storeIndex} ok`);
                new google.maps.Marker({
                        position: results[0].geometry.location,
                        map,
                        title: name,
                        icon: {
                            url: isWarehouse === true ? icons.warehouseStore.icon : icons.regularStore.icon,
                            scaledSize: new google.maps.Size(40, 40)
                        }
                    }
                );
            } else if (status === "OVER_QUERY_LIMIT") {
                delay += 100;
                console.log(`OVER_QUERY_LIMIT, delay is increased to ${delay}`);
                console.log(`store ${storeIndex} failed`);
            }
        });
}





$("ul li").click(function () {
    $(this).addClass("active");
/*todo: add center map to active store*/
    $(this).parent().children("li").not(this).removeClass("active");
});
