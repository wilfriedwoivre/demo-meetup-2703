(function () {
    angular.module('gameApp')
        .config(['$locationProvider', '$routeProvider',
            function ($locationProvider, $routeProvider) {
                $locationProvider.hashPrefix('!');
                $routeProvider
                    .when('/session', { template: '<session></session>' })
                    .when('/game/:id', { template: '<game></game>' })
                    .otherwise('/session');
            }]);
})();