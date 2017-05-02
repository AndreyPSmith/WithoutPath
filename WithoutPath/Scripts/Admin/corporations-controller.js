var CorporationsApp = angular.module('CorporationsApp', []);

CorporationsApp.directive('uiPagination', function () {

    return {
        template:
            '<div class="pagination" id="PaginationWrapper">' +
            '<ul>' +

            '<li ng-if="PageableData.CurrentPage == 1" class="active"><span><i class="fa fa-backward"></i></span></li>' +
            '<li ng-if="PageableData.CurrentPage != 1" ><span class="PaginationControl" ng-click="GoToPage(\'Previous\')"><i class="fa fa-backward"></i></span></li>' +

            '<li ng-repeat="no in PageableData.PagesNo" ng-class="{active: no.val == PageableData.CurrentPage, PaginationControl : no.val != PageableData.CurrentPage}" ng-click="GoToPage(no.val)"><span>{{no.val}}</span></li>' +

            '<li ng-if="PageableData.CurrentPage == PageableData.LastPage" class="active"><span><i class="fa fa-forward"></i></span></li>' +
            '<li ng-if="PageableData.CurrentPage != PageableData.LastPage"><span class="PaginationControl" ng-click="GoToPage(\'Next\')"><i class="fa fa-forward"></i></span></li>' +

            '<li><input class="go-to-page-text" min="1" class="small" type="number" ng-model="PageableData.NewPage" ng-keydown="IsEnteredNum(event=$event)"></li>' +
            '<li><span  class="go-to-page" ng-click="GoNewPage()"><i class="fa fa-fast-forward"></i></span></li>' +

            '</ul>' +
            '</div>' +
            '<div class="btn-group dropdown">' +
            '  <input type="text" class="dropdown-text" readonly value="{{PageableData.ItemPerPage}}" />' +
            '  <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" ng-click="ShowDropdownList($event);">' +
            '  <span class="caret"></span>' +
            '  </button>' +
            '  <ul ng-if="PageableData.IsShowDropdown" class="dropdown-menu">' +
            '  <li ng-repeat="page in PageableData.ItemsPerPage" ng-click="setPage(page)">{{page}}</li>' +
            '  </ul>' +
            '</div>',
        link: function (scope, iElement, iAttrs) {

            scope.setPage = function (page) {
                scope.PageableData.ItemPerPage = page;
                scope.PageableData.IsShowDropdown = false;
                scope.GoToPage(1);
            };

            scope.ShowDropdownList = function (event) {
                scope.PageableData.IsShowDropdown = !scope.PageableData.IsShowDropdown;
            };

            scope.CalculatePagesNo = function () {
                var totalPages = Math.ceil(scope.PageableData.TotalRecords / scope.PageableData.ItemPerPage);
                scope.PageableData.LastPage = Math.ceil(scope.PageableData.TotalRecords / scope.PageableData.ItemPerPage);

                scope.PageableData.PagesNo = new Array();
                //По порядку
                for (var i = 1; i <= totalPages; i++) {
                    //Условие что выводим только необходимые номера
                    if (((i <= 2) || (i > (totalPages - 2))) || ((i > (scope.PageableData.CurrentPage - 2)) && (i < (scope.PageableData.CurrentPage + 2)))) {
                        scope.PageableData.PagesNo.push({ val: i.toString() });
                    }
                    else if ((i == 3) && (scope.PageableData.CurrentPage > 3)) {
                        //Троеточие первое
                        scope.PageableData.PagesNo.push({ val: "..." });
                    }
                    else if ((i == (totalPages - 2)) && (scope.PageableData.CurrentPage < (totalPages - 2))) {
                        //Троеточие второе
                        scope.PageableData.PagesNo.push({ val: "..." });
                    }
                }
            };

            scope.GoNewPage = function () {
                scope.GoToPage(scope.PageableData.NewPage);
            };

            scope.GoToPage = function (page) {

                if (page == "...")
                    return;

                var lim = Math.ceil(scope.PageableData.TotalRecords / scope.PageableData.ItemPerPage);

                if (page == "Next") {
                    if (lim == scope.PageableData.CurrentPage)
                        return;
                    scope.PageableData.CurrentPage++;
                }
                else if (page == "Previous") {
                    if (scope.PageableData.CurrentPage == 1)
                        return;

                    scope.PageableData.CurrentPage--;
                }
                else {
                    var num = parseInt(page);
                    if (isNaN(num) ||
                        num > lim ||
                        num < 1) {
                        scope.PageableData.NewPage = scope.PageableData.CurrentPage;
                        return;
                    }
                    scope.PageableData.CurrentPage = num;
                }

                scope.PageableData.NewPage = scope.PageableData.CurrentPage;
                scope.GetList();
            };
        }
    };
});

CorporationsApp.controller('CorporationCtrl',['$scope', '$http', '$q', function ($scope, $http, $q) {

    $scope.apiCorporations = "/api/Corporations";

    $scope.PageableData = {
        PagesNo: [],
        TotalRecords: 0,
        IsShowDropdown: false,
        ItemPerPage: 20,
        LastPage: 1,
        CurrentPage: 1,
        NewPage: 1,
        ItemsPerPage: [10, 20, 50, 100],
        List: [],
        SearchString: ""
    };

    $scope.Initialize = function () {
        $scope.GetList();
    };

    $scope.Save = function () {

        var requests = [];
        $.each($scope.PageableData.List, function (index, item) {

            var data = {
                Id: item.Id,
                Name: item.Name,
                EveID: item.EveID,
                Ticker: item.Ticker,
            };

            if (item.IsNew) {
                requests.push($http({
                    method: 'POST',
                    url: $scope.apiCorporations,
                    data: data
                }));
            }
            else if (item.IsEdit) {
                requests.push($http({
                    method: 'PUT',
                    url: $scope.apiCorporations + "(" + item.Id + ")",
                    data: data
                }));
            }
            else if (item.IsToDelete) {
                requests.push($http({
                    method: 'DELETE',
                    url: $scope.apiCorporations + "(" + item.Id + ")"
                }));
            }
        });

        $q.all(requests).then(function (responses) {
            var errors = new Array();
            $.each(responses, function (index, response) {
                if (!response.status.toString().startsWith('20'))
                    errors.push("Ошибка во время изменения корпорации - " + response.config.data.Id);
            });

            if (errors.length > 0) $scope.ShowError(errors);
            else $scope.GetList();
        });
    };

    $scope.SetEditState = function (item) {

        if (item.IsToDelete)
            item.IsToDelete = false;

        item.IsEdit = !item.IsEdit;

        if (item.IsNew && !item.IsEdit) {
            var index = $scope.PageableData.List.indexOf(item);
            $scope.PageableData.List.splice(index, 1);
        }
        else if (!item.IsNew) {
            if (item.IsEdit) {
                item.backup = $.extend({}, item);
            }
            else {
                var index = $scope.PageableData.List.indexOf(item);
                item = item.backup;
                item.IsEdit = !item.IsEdit;
                delete item.backup;
                $scope.PageableData.List.splice(index, 1, item);
            }
        }
    };

    $scope.SetDeleteState = function (item) {

        if (!item.IsNew)
            item.IsToDelete = !item.IsToDelete;
    };

    $scope.Add = function () {

        var item = {
            IsEdit: true,
            IsNew: true,
            IsToDelete: false,
            Name: "",
            Ticker: "",
            EveID: 0,
            Id: 0
        };

        $scope.PageableData.List.unshift(item);
    };

    $scope.FormatDate = function (val) {
        if (!val)
            return null;

        if (val.toString().substring(0, 5) === "/Date") {
            var date = new Date(2000, 1);
            if (val[6] != "-")
                date = new Date(parseInt(val.substr(6)));
            return date.format("dd.mm.yyyy");
        }
        else if (val.split("-").length > 0) {
            var arr = val.split("-");
            return new Date(parseInt(arr[0]), parseInt(arr[1]) - 1, arr[2].substr(0, 2)).format("dd.mm.yyyy");
        }

        return val;
    };

    $scope.Search = function () {
        $scope.PageableData.NewPage = $scope.PageableData.CurrentPage = 1;
        $scope.GetList();
    }

    $scope.GetList = function () {
        $scope.PageableData.List = [];

        var url = $scope.apiCorporations + "?";

        if ($scope.PageableData.SearchString)
            url += "$filter=contains(Name, '" + $scope.PageableData.SearchString + "') or " +
                           "contains(Ticker, '" + $scope.PageableData.SearchString + "')&";

        url += "$count=true&";

        url += "$top=" + $scope.PageableData.ItemPerPage + "&";
        var skip = ($scope.PageableData.CurrentPage - 1) * $scope.PageableData.ItemPerPage;
        if (skip > 0)
            url += "$skip=" + skip + "&";

        url = url.substring(0, url.length - 1);

        $http.get(url, { cache: false })
             .then(function (responce) {
                 if (responce.status == 200) {
                     $scope.PageableData.TotalRecords = parseInt(responce.data['@odata.count']);
                     $scope.PageableData.List = responce.data.value;
                     $scope.CalculatePagesNo();
                 }
             })
             .catch(function () {
                 $scope.ShowError("Ошибка при получении данных!");
             });

    };


    $scope.IsEnteredNum = function (event) {
        if ($.inArray(event.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Ctrl+A, Command+A
          (event.keyCode === 65 && (event.ctrlKey === true || event.metaKey === true)) ||
            // home, end, left, right, down, up
          (event.keyCode >= 35 && event.keyCode <= 40)) {
            return;
        }

        if ((event.shiftKey || (event.keyCode < 48 || event.keyCode > 57)) && (event.keyCode < 96 || event.keyCode > 105)) {
            event.preventDefault();
        }
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
    angular.bootstrap(document, ['CorporationsApp']);
});
