(function () {
    angular.module('storeApp')
        .config(['$locationProvider', '$routeProvider',
            function ($locationProvider, $routeProvider) {
                $locationProvider.hashPrefix('!');
                $routeProvider
                    .when('/about', { template: '<about></about>' })
                    .when('/categories', { template: '<categories></categories>' })
                    .when('/categories/:id', { template: '<products></products>' })
                    .when('/cart', { template: '<cart></cart>' })
                    .when('/wishlist', { template: '<wishlist></wishlist>' })
                    .otherwise('/about');
            }]);
})();