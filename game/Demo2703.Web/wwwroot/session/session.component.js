(function () {
    angular.module('session')
        .component('session', {
            templateUrl: 'session/session.template.html',
            controller: ['$http', '$location', SessionContoller]
        });

    function SessionContoller($http, $location) {
        var ctrl = this;
        ctrl.difficulty = 0;
        ctrl.name = "";
        ctrl.id = ""; 

        ctrl.login = login;


        function login() {
            $http({
                method: 'POST',
                url: 'api/session/',
                data: JSON.stringify({
                    "name": ctrl.name,
                    "difficulty": ctrl.difficulty
                }),
                headers:
                { 'Content-Type': 'application/json' }
            }).then(function (response) {
                ctrl.id = response.data.id;
                var newRoute = '/game/' + ctrl.id;

                $location.path(newRoute);
            });
        }
    }
})();