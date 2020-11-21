const iconBase =
    "https://maps.google.com/mapfiles/kml/pushpin/";
const icons = {
    regularStore: {
        icon: iconBase + "red-pushpin.png",
    },
    websiteStore: {
        icon: iconBase + "ylw-pushpin.png",
    }
};

var storeIndex = 0;
var map;
var geocoder;

function initMap() {
     map = new google.maps.Map(document.getElementById("map"),
        {
            zoom: 15,
            center: { lat: 32.086813, lng: 34.77630 }
        });
     geocoder = new google.maps.Geocoder();

    while (storeIndex < storesObject.length) {
        getStoreAddress(storesObject[storeIndex], storeIndex);
        storeIndex++;
    };
}


function getStoreAddress(store, storeIndex) {
    let [address, name, isWebsite] = store;
    //console.log(`store: ${name} index:${storeIndex}`);
    geocoder.geocode({ address: address },
        (results, status) => {
            if (status === "OK") {
                console.log(`store ${storeIndex} ok`);
                new google.maps.Marker({
                        position: results[0].geometry.location,
                        map,
                        title: name,
                        icon: {
                            url: isWebsite === "true" ? icons.websiteStore.icon : icons.regularStore.icon,
                            scaledSize: new google.maps.Size(40, 40)
                        }
                    }
                );
            } else if (status === "OVER_QUERY_LIMIT") {
                //console.log(`store ${storeIndex} failed`);
            }
        });
}





$("ul li").click(function () {
    $(this).addClass("active");

    $(this).parent().children("li").not(this).removeClass("active");
    var storeAddress = $(this).find(".store_address")[0].innerText.toString();

    geocoder.geocode({ address: storeAddress },
        (results, status) => {
            if (status === "OK") {
                map.setCenter(results[0].geometry.location);
            } else {
                console.log(`error recentering the map to ${storeAddress}.`);
            }
        });
});

