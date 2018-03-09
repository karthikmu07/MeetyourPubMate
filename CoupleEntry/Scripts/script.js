
var map;
var lastMarker;
var mainModule = angular.module("mainModule", []);

mainModule.run(function ($rootScope, $location) {
    $rootScope.showMask = false;
    $rootScope.showModal = true;
    $rootScope.signOut = function () {

        gapi.auth2.init();
        gapi.load('auth2', function () {
            var auth2 = gapi.auth2.getAuthInstance();
            auth2.signOut().then(function () {
                RedirectToLogin();
            });
        });


    }
    $rootScope.ShowMyModal = function () {
        $rootScope.showModal = true;
        angular.element('#myLoginModal').addClass("block");
    }
    $rootScope.HideMyModal = function () {
        $rootScope.showModal = false;
    }
});

function GetUserInfo($http, $rootScope) {
    $http.get("./Home/GetUserInfo")
        .then(function successCallback(response) {
            $rootScope.MyInfo = response.data;
        }, function errorCallback(response) {
            console.log("Unable to perform get request");
        });

}

function Initialize($scope, $http, $rootScope) {
    $scope.distanceFilter = 50;
    $scope.genderFilter = "Both";
    $scope.HaveMatches = false;
    $scope.peopleList = [];
    $scope.yourAddress = "";
    GetUserInfo($http, $rootScope);
    gapi.load('auth2', function () {
        gapi.auth2.init();
    });
}

function GetMyLocation($scope) {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition($scope.SetMyPosition, ShowError, { maximumAge: Infinity, timeout: 60000, enableHighAccuracy: false });
    } else {
        x.innerHTML = "Geolocation is not supported by this browser.";
    }

}

function ShowOnMap(lat, lon, label, address) {

    var latLng = new google.maps.LatLng(lat, lon);
    var marker = new google.maps.Marker({
        position: latLng,
        map: map,
        label: label
    });

    map.setCenter(latLng)
    var infowindow = new google.maps.InfoWindow({
        content: address
    });
    marker.addListener('click', function () {
        infowindow.open(map, marker);
    });
    if (label != "You")
        lastMarker = marker;


}

function UpsertUserPosition(position, $http) {
    $http.get("./Home/AddUserPositionToDB?latitude=" + position.coords.latitude + "&longitude=" + position.coords.longitude)
        .then(function successCallback(response) {

        }, function errorCallback(response) {
            console.log("Unable to perform get request");
        });

}

function FindNearbyPeople($scope, users, $rootScope) {

    var destinations = [], origins = [];
    var origin = new google.maps.LatLng($scope.myLat, $scope.myLong);
    users.sort(CompareLastSeen);
    $scope.usersOnline = users;
    origins[0] = origin;
    for (var i = 0; i < users.length; i++) {
        destinations[i] = new google.maps.LatLng(users[i].Latitude, users[i].Longitude);
    }

    if (origins.length > 0 && destinations.length > 0) {
        var service = new google.maps.DistanceMatrixService();
        service.getDistanceMatrix(
            {
                origins: origins,
                destinations: destinations,
                travelMode: 'DRIVING',
                unitSystem: google.maps.UnitSystem.METRIC,
            }, $scope.callback);
    }
    else {
        $rootScope.showMask = false;
        alert('Sorry, No one is nearby!');
    }
}

function CompareLastSeen(a, b) {
    if (parseInt(a.LastSeenDiff) < parseInt(b.LastSeenDiff))
        return -1;
    if (parseInt(a.LastSeenDiff) > parseInt(b.LastSeenDiff))
        return 1;
    return 0;
}

function CalculateLastSeen(difference) {

    var lastSeen = difference + " seconds ago.";
    if (difference > 59) {
        difference = Math.floor(difference / 60);
        lastSeen = difference + " minutes ago.";

        if (difference > 59) {
            difference = Math.floor(difference / 60);
            lastSeen = difference + " hours ago.";

            if (difference > 23) {
                difference = Math.floor(difference / 24);
                lastSeen = difference + " days ago.";

                if (difference > 29) {
                    difference = Math.floor(difference / 30);
                    lastSeen = difference + " months ago.";
                }
            }
        }
    }
    return lastSeen;
}

function ShowError(error) {
    var x = document.getElementById("error-div");
    switch (error.code) {
        case error.PERMISSION_DENIED:
            x.innerHTML = "User denied the request for Geolocation."
            break;
        case error.POSITION_UNAVAILABLE:
            x.innerHTML = "Location information is unavailable."
            break;
        case error.TIMEOUT:
            x.innerHTML = "The request to get user location timed out."
            break;
        case error.UNKNOWN_ERROR:
            x.innerHTML = "An unknown error occurred."
            break;
    }
}

function AddOrRemoveNewLike(person, liked, $http, $scope, $rootScope) {
    var targetId = person.UserId.toString();
    $http.get("./Home/AddOrRemoveLike?targetId=" + person.UserId + "&liked=" + liked)
        .then(function successCallback(response) {
            if (response.status != 200) {
                location.reload();
            }
            else {
                if (liked) {
                    $rootScope.MyInfo.Likes.push(targetId);
                    if (response.data) {
                        $rootScope.MyInfo.Matches.push(targetId);
                        person.ShowInMatched = true;
                        if ($rootScope.MyInfo.Matches.length == 1)
                            $scope.HaveMatches = true;
                        alert("YAY! Its a Match!");
                    }
                }
                else {
                    var index = $rootScope.MyInfo.Likes.indexOf(targetId);
                    if (index > -1) {
                        $rootScope.MyInfo.Likes.splice(index, 1);
                    }
                    index = $rootScope.MyInfo.Matches.indexOf(targetId);
                    if (index > -1) {
                        person.ShowInMatched = false;
                        $rootScope.MyInfo.Matches.splice(index, 1);
                        if ($rootScope.MyInfo.Matches.length == 0)
                            $scope.HaveMatches = false;
                    }
                }

            }
        }, function errorCallback(response) {
            console.log("Unable to perform get request");
        });


}

mainModule.controller('loginController', function ($scope, $http) {

});

mainModule.controller('indexController', function ($scope, $http, $rootScope) {
    Initialize($scope, $http, $rootScope);
    $scope.SetMyPosition = function (position) {
        $scope.myLat = position.coords.latitude;
        $scope.myLong = position.coords.longitude;
        map = new google.maps.Map(document.getElementById('map'), {
            zoom: 14,
            center: { lat: $scope.myLat, lng: $scope.myLong }
        });
        ShowOnMap($scope.myLat, $scope.myLong, "You");
        UpsertUserPosition(position, $http);
    }

    GetMyLocation($scope);
    setInterval(function () {
        GetMyLocation($scope);
    }, 120000);

    $scope.GetOtherUsers = function () {
        $rootScope.showMask = true;
        $http.get("./Home/GetOtherUsers")
            .then(function successCallback(response) {
                if (response.status == 200) {
                    FindNearbyPeople($scope, response.data, $rootScope);
                }
                else {
                    location.reload();
                }

            }, function errorCallback(response) {
                console.log("Unable to perform get request");
            });
    }

    $scope.GetGenderClass = function (gender) {
        var cls = "panel cursorPointer tile panel-";
        if (gender == "Male")
            return cls + "info";
        else
            return cls + "danger";
    }

    $scope.GetLikeClass = function (UserId) {
        if ($rootScope.MyInfo.Likes.indexOf(UserId.toString()) > -1)
            return "pull-right glyphicon glyphicon-heart heartIcon";
        else
            return "pull-right glyphicon glyphicon-heart-empty heartIcon";
    }

    $scope.callback = function (response, status) {
        $scope.yourAddress = "Your Address:" + response.originAddresses[0];
        if (response.rows.length > 0) {

            $scope.usersOnline.forEach(AddGeoData);

            function AddGeoData(person, index) {
                person.Address = response.destinationAddresses[index];
                person.Distance = response.rows[0].elements[index].distance;
                person.Duration = response.rows[0].elements[index].duration;
                person.LastSeen = CalculateLastSeen(parseInt(person.LastSeenDiff));
                person.ShowInNearby = false;
                if (person.Distance && person.Distance.value / 1000 <= $scope.distanceFilter && ($scope.genderFilter == "Both" || $scope.genderFilter == person.Gender)) {
                    person.ShowInNearby = true;
                }
                person.ShowInMatched = false;
                if ($rootScope.MyInfo.Matches.indexOf(person.UserId.toString()) > -1) {
                    person.ShowInMatched = true;
                    $scope.HaveMatches = true;
                }
            }

            $scope.peopleList = $scope.usersOnline;

        }
        $rootScope.showMask = false;
        $scope.$apply();
    }

    $scope.ShowPersonOnMap = function (person) {
        if (lastMarker)
            lastMarker.setMap(null);
        ShowOnMap(person.Latitude, person.Longitude, person.Name, person.Address);
    }

    $scope.LikeUnlike = function (person, objSender) {
        var element = angular.element(objSender.target);
        var liked = element.hasClass("glyphicon-heart-empty");
        AddOrRemoveNewLike(person, liked, $http, $scope, $rootScope);
    }


});