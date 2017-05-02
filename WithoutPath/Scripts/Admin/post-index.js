function PostIndex() {
    var _this = this;

    _this.PostToDeleteID = 0;

    this.init = function () {

        $(document).on("click", "#SearchSubmit", function () {
            _this.Search();
        });

        $(document).on("keypress", "#SearchText", function (e) {
            if (e.keyCode === 13) {
                _this.Search();
            }
        });

        $(document).on("click", ".fa-trash", function () {
            _this.DeleteSubmit($(this).attr("postid"));
        });

        $(document).on("click", "#DeletePost", function () {
            $("#PostID").val(_this.PostToDeleteID);
            document.forms["DeleteForm"].submit();
        });
      
    };

    this.Search = function () {

        var searchText = $("#SearchText").val();
        $("#SearchString").val(searchText);

        document.forms["SearchForm"].submit();

    };

    this.DeleteSubmit = function (ID) {
        var text = "";
        text += "<div class='modal fade' tabindex='-1' role='dialog' aria-hidden='true'>";
        text += " <div class='modal-header'>";
        text += "      <button type='button' class='close' data-dismiss='modal' aria-hidden='true'><i class='fa fa-times'></i></button>";
        text += "  </div>";
        text += "<div class='modal-body'>";
        text += "<i class='fa fa-exclamation-triangle'></i> <h3 class='inline'>Удалить пост?</h3>";
        text += "</div><div class='modal-footer'>";
        text += "<button id='DeletePost' class='btn btn-primary btn-login-popap'>Ok</button></div></div>";

        _this.PostToDeleteID = ID;

        $(".modal-backdrop").remove();
        var popupWrapper = $("#PopupWrapper");
        popupWrapper.empty();
        popupWrapper.html(text);
        $(".modal", popupWrapper).modal();
    };

    this.ShowError = function (Messages) {
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
}

var postIndex = null;
$().ready(function () {
    postIndex = new PostIndex();
    postIndex.init();
});