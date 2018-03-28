(function () {
    angular.module('about')
        .component('about', {
            templateUrl: 'modules/about/about.template.html',
            controller: ['$http', aboutController]
        });

    function aboutController($http) {
        var ctrl = this;

    }

})();