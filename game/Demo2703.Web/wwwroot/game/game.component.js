(function () {
    angular.module('game')
        .component('game', {
            templateUrl: 'game/game.template.html',
            controller: ['$routeParams', '$http', GameController]
        });

    function GameController($routeParams, $http) {
        var ctrl = this;
        ctrl.id = ""; 
        ctrl.totalScore = -1;
        ctrl.refreshScore = refreshScore;
        ctrl.getGame = getGame;
        ctrl.validResult = validResult;
        ctrl.first = -1;
        ctrl.second = -1;
        ctrl.operand = "";
        ctrl.result = 0; 

        refreshScore();
        getGame();

        function refreshScore() {
            $http.get(`api/session/${$routeParams.id}`)
                .then(function (response) {
                    ctrl.totalScore = response.data.totalScore;
                });
        }

        function getGame() {
            ctrl.result = 0;
            $http.post(`api/game/${$routeParams.id}`, "")
                .then(function (response) {
                    ctrl.id = response.data.id;
                    ctrl.first = response.data.numberA;
                    ctrl.second = response.data.numberB;
                    var operand = response.data.operand;
                    switch (operand) {
                        case 0:
                            ctrl.operand = '+';
                            break;
                        case 1:
                            ctrl.operand = '-';
                            break;
                        case 2:
                            ctrl.operand = '*';
                            break;
                        default:
                    }
                });
        }

        function validResult() {
            $http.post(`api/game/valid/${$routeParams.id}/${ctrl.id}`, ctrl.result)
                .then(function(response) {
                    refreshScore();
                    getGame();
                });

        }


    }
})();