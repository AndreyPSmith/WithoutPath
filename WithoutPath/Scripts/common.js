function Common() {
    var _this = this;

    this.init = function () {

        $('[data-toggle="tooltip"]').tooltip(); 

        $("#LoginPopup").click(function () {
          _this.showPopup("/Account/AjaxLogin", initLoginPopup);
        });

        $("#LoginPopup").mouseover(function () {
            $("#hamburger-plus").addClass("is-active");
        });

        $("#LoginPopup").mouseout(function () {
            $("#hamburger-plus").removeClass("is-active");
        });

        $("#LogOut").mouseover(function () {
            $("#hamburger-cross").addClass("is-active");
        });

        $("#LogOut").mouseout(function () {
            $("#hamburger-cross").removeClass("is-active");
        });

    };
    this.showPopup = function (url, callback) {
        $.ajax({
            type: "GET",
            url: url,
            success: function (data) {
                showModalData(data, callback);
            }
        });
    };

    function initLoginPopup(modal) {
        $("#LoginButton").click(function () {
            $.ajax({
                type: "POST",
                url: "/Account/AjaxLogin",
                data: $("#LoginForm").serialize(),
                success: function (data) {
                    showModalData(data);
                    initLoginPopup(modal);
                }
            });
        });
    }

    function showModalData(data, callback) {
        $(".modal-backdrop").remove();
        var popupWrapper = $("#PopupWrapper");
        popupWrapper.empty();
        popupWrapper.html(data);
        var popup = $(".modal", popupWrapper);
        $(".modal", popupWrapper).modal();
        if (callback != undefined) {
            callback(popup);
        }
    }
}

var common = null;
$().ready(function () {
    common = new Common();
    common.init();
});
