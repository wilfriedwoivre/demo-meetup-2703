(function () {
    angular.module('cartPreview')
        .component('cartPreview', {
            templateUrl: 'modules/cartPreview/cartPreview.template.html',
            controller: ['$scope', 'Cart', CartPreviewController]
        });

    function CartPreviewController($scope, Cart) {
        var ctrl = this;
        ctrl.count = 0;
        $scope.$on('cart:updated',
            function() {
                ctrl.count = Cart.count;
            });
    };
})();