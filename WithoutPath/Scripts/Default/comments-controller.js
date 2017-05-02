var CommentsApp = angular.module('CommentsApp', ['ui.bootstrap']);

CommentsApp.controller('CommentsCtrl', ['$scope', '$http',  '$uibModal', function ($scope, $http, $uibModal) {

    $scope.apiComments = "/api/Comments";

    $scope.PageData = {
        PostID: -1,
        MainID: -1,
        IsAdmin: false,
        Comments: [],
        NewCommentContent: ""
    };

    $scope.Initialize = function () {
        $scope.PageData.PostID = parseInt($("#PostID").val());
        $scope.PageData.IsAdmin = $("#IsAdmin").val() == "True";
        $scope.PageData.MainID = parseInt($("#MainID").val());

        $scope.GetComments();
    };

    $scope.GetComments = function () {
       
        var url = $scope.apiComments + "?$filter=PostID eq " + $scope.PageData.PostID + "&$count=true&$expand=Character";
        $http.get(url, { cache: false })
             .then(function (responce) {
                 if (responce.status == 200) {

                     $scope.PageData.Comments = [];

                     $.each(responce.data.value, function (index, comment) {
                         var date = new Date(comment.AddedDate);
                         comment.Date = date.getDate() + "." + (date.getMonth() + 1) + "." + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes();
                         comment.IsCanEdit = comment.CharacterID == $scope.PageData.MainID || $scope.PageData.IsAdmin;

                         $scope.PageData.Comments.push(comment);
                     });
                 }
             })
             .catch(function () {
                 $scope.ShowError("Ошибка при получении данных!");
             });
    };

    $scope.DeleteComment = function (comment) {

        var template = "";
        template += " <div class='modal-header'>";
        template += "      <button  ng-click='close()' type='button' class='close' data-dismiss='modal' aria-hidden='true'><i class='fa fa-times'></i></button>";
        template += "  </div>";
        template += "<div class='modal-body'>";
        template += "<i class='fa fa-exclamation-triangle'></i> <h3 class='inline'>Удалить комментарий?</h3>";
        template += "</div><div class='modal-footer'>";
        template += "<button ng-click='Delete()' class='btn btn-primary btn-login-popap'>Ok</button></div>";

        var modalInstance = $uibModal.open({
            animation: true,
            template: template,
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            size: 'sm',
            scope: $scope
        });

        $scope.close = function () {

            var $parent = $(".modal");
            $parent.removeClass('in')

            function removeElement() {
                modalInstance.dismiss();
            }

            $.support.transition && $parent.hasClass('fade') ?
              $parent
                .one($.support.transition.end, removeElement)
                .emulateTransitionEnd(150) :
              removeElement()
        };

        $scope.Delete = function () {
            $http({
                method: 'DELETE',
                url: $scope.apiComments + "(" + comment.Id + ")"
            }).then(function (responce) {
                if (responce.status == 204) {
                    $scope.GetComments();
                }
                $scope.close();
            })
             .catch(function () {
                 $scope.close();
                 $scope.ShowError("Ошибка при получении данных!");
             });
        };
    };

    $scope.UndoComment = function (comment) {
        comment.IsEdit = false;
        comment.Content = comment.backup;
    };
    $scope.EditComment = function (comment) {
        comment.IsEdit = true;
        comment.backup = comment.Content;
    };

    $scope.UpdateComment = function (comment) {

        var data = {
            Id: comment.Id,
            PostID: comment.PostID,
            Content: comment.Content,
            AddedDate: (new Date()).toJSON(),
            CharacterID: comment.CharacterID
        };
        $http({
            method: 'PUT',
            url: $scope.apiComments + "(" + comment.Id + ")",
            data: data
        }).then(function (responce) {
            if (responce.status == 204) {
                $scope.GetComments();
            }
        }).catch(function () {
            $scope.ShowError("Ошибка при изменении!");
        });
    };

    $scope.AddComment = function () {

        var data = {
            Id: 0,
            PostID: $scope.PageData.PostID,
            Content: $scope.PageData.NewCommentContent,
            AddedDate: (new Date()).toJSON(),
            CharacterID: 0
        };

        $http({
            method: 'POST',
            url: $scope.apiComments,
            data: data
        }).then(function (responce) {
            if (responce.status == 201) {
                $scope.GetComments();
                $scope.PageData.NewCommentContent = "";
            }
        }).catch(function () {
            $scope.ShowError("Ошибка при добавлении!");
        });

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
    angular.bootstrap(document, ['CommentsApp']);
});
