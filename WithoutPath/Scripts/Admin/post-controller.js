var PostApp = angular.module('PostApp', ['textAngular']);

PostApp.directive('uiPreview', function () {
    return {
        template:

            '<div class="post-form-group">' +
                '<label class="col-md-2 control-label" >Заголоовк:</label>' +
                '<div class="col-md-10">' +
                    '<textarea ng-keyup="setWidget();"  style="resize:vertical;" ng-model="Header" rows="3"></textarea>' +
                    '<span class="field-validation-valid text-danger" data-valmsg-for="Password" data-valmsg-replace="true"></span>' +
                '</div>' +
            '</div>' +

            '<div class="post-form-group">' +
                '<label class="col-md-2 control-label">Фон:</label>' +
                '<div class="col-md-10">' +
                    '<input class="form-control"  ng-model="imageLink"  type="text"> <i class="fa fa-refresh" ng-click="setWidget();"></i>' +
                    '<span class="field-validation-valid text-danger" data-valmsg-for="Password" data-valmsg-replace="true"></span>' +
                '</div>' +
            '</div>' +

            '<div class="checkboxes"><input type="checkbox"  ng-model="IsFixed" />' +
            '<label class="bool-label" ng-click="CheckFixed();">{{(IsFixed) ? "Закреплено" : "Не закреплено"}}</label><br />' +

            '<input type="checkbox"  ng-model="IsInternal" />' +
            '<label class="bool-label" ng-click="CheckInternal();">{{(IsInternal) ? "Для своих" : "Для всех"}}</label>' +

            '<ul class="demo-size-ul">' +
            '<li><input type="radio" id="Small"  ng-model="size" value="small" ng-click="setWidget();">' +
            '<label class="label-for" for="Small"> Small</label></li>' +

            '<li><input type="radio" id="Medium"  ng-model="size" value="medium" ng-click="setWidget();">' +
            '<label class="label-for" for="Medium"> Medium</label></li>' +

            '<li><input type="radio" id="Large" ng-model="size" value="large" ng-click="setWidget();">' +
            '<label class="label-for" for="Large"> Large</label></li></ul></div>' +

            '<div class="grid-stack" id="DemoGrid">' +

            '</div>',
        link: function (scope, iElement, iAttrs) {

            $('.grid-stack').gridstack({
                float: false,
                disableDrag: true,
                disableResize: true
            });

            scope.grid = $('.grid-stack').data('gridstack');

            scope.CheckFixed = function () {
                scope.IsFixed = !scope.IsFixed;
                scope.setWidget();
            }

            scope.CheckInternal = function () {
                scope.IsInternal = !scope.IsInternal;
                scope.setWidget();
            }

            scope.setWidget = function () {
                scope.grid.removeAll();

                var content = '<div>';

                if (scope.imageLink && scope.imageLink !== '' && scope.imageLink !== 'http://' && scope.imageLink !== 'https://')
                    content += '<figure class="grid-stack-item-content"  style="background-image: url(' + scope.imageLink + ');">';
                else
                    content += '<figure class="grid-stack-item-content" style="background-image: url(../../Content/images/evemail.svg);">';

                content += '<figcaption>';

                if (scope.IsFixed) {
                    content += '<i class="fa fa-thumb-tack"></i>';
                }

                if (scope.IsInternal) {
                    content += '<i class="fa fa-eye-slash"></i>';
                }

                content += '<span class="header-preview">' + scope.Header + '</span></figcaption></figure></div>';

                if (scope.size == "small") {
                    scope.Width = 3;
                    scope.Height = 3;
                }
                else if (scope.size == "medium") {
                    scope.Height = 3;
                    scope.Width = 6;
                }
                else if (scope.size == "large") {
                    scope.Width = 6;
                    scope.Height = 6;
                }

                scope.grid.addWidget($(content), 0, 0, scope.Width, scope.Height, 'yes');

            };

            scope.setWidget();
        }
    };
});

PostApp.controller('PostCtrl', ['$scope', '$http', '$q', 'textAngularManager',function ($scope, $http, $q, textAngularManager) {

    $scope.PageableData = {
        modalShown: false
    };

    $scope.Initialize = function () {

        $scope.Id = $("#Id").val();

        $scope.htmlContent = $("input#Content").val();
        if (!$scope.htmlContent)
            $scope.htmlContent = "";

        $scope.Header = $("#Header").val();

        $scope.Height = $("#Height").val();
        if (!$scope.Height)
            $scope.Height = 3;

        $scope.Width = $("#Width").val();
        if (!$scope.Width)
            $scope.Width = 3;

        $scope.size = "small";

        if ($scope.Width == 6)
            $scope.size = $scope.Height == 6 ? "large" : "medium";

        $scope.IsFixed = $("#IsFixed").val() && $("#IsFixed").val() == "True";
        $scope.IsInternal = $("#IsInternal").val() && $("#IsInternal").val() == "True";

        $scope.imageLink = $("#Picture").val();
        if (!$scope.imageLink)
            $scope.imageLink = "http://";

    };

    $scope.toggleModal = function () {
        $scope.PageableData.modalShown = !$scope.PageableData.modalShown;
    };

    $scope.Save = function () {

        $("#Id").val($scope.Id);
        $("input#Content").val($scope.htmlContent);
        $("#Header").val($scope.Header);
        $("#Height").val($scope.Height);
        $("#Width").val($scope.Width);
        $("#IsFixed").val($scope.IsFixed)
        $("#IsInternal").val($scope.IsInternal)

        var link = ($scope.imageLink == "http://" || !$scope.imageLink) ? null : $scope.imageLink;
        $("#Picture").val(link);

        document.forms["EditForm"].submit();
    };

    $scope.ShowError = function (Messages) {
        var text = "";

        text += "<div class='modal fade' tabindex='-1' role='dialog' aria-hidden='true'>";
        text += " <div class='modal-header'>";
        text += "      <button type='button' class='close' data-dismiss='modal' aria-hidden='true'><i class='fa fa-times'></i></button>";
        text += "  </div>";
        text += "<div class='modal-body'>";

        if (typeof Messages != "string") {
            text += "<ul>";
            for (var i = 0; i < Messages.length; i++) {
                text += "<li>" + Messages[i] + "</li>";
            }
            text += "</ul>";
        }
        else
            text += Messages;

        text += " </div>";
        text += "   <div class='modal-footer'>";
        text += " </div>";
        text += " </div>";

        $(".modal-backdrop").remove();
        var popupWrapper = $("#PopupWrapper");
        popupWrapper.empty();
        popupWrapper.html(text);
        $(".modal", popupWrapper).modal();
    };

    $scope.Initialize();

}]);

angular.element(document).ready(function () {
    angular.bootstrap(document, ['PostApp']);
});
