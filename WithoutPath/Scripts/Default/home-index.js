function HomeIndex() {
    var _this = this;

    _this.PageableData = {
        Skip: 0,
        Take: 10,
        apiPosts: "/api/Posts"
    };


    this.init = function () {

        $('.grid-stack').gridstack({
            float: false,
            disableDrag: true,
            disableResize: true
        });

        _this.grid = $('.grid-stack').data('gridstack');
        _this.GetPosts();
    };

    this.GetPosts = function () {

        var url = _this.PageableData.apiPosts + "?$orderby=IsFixed desc&$count=true&";

        url += "&$top=" + _this.PageableData.Take + "&";
        if (_this.PageableData.Skip > 0)
            url += "$skip=" + skip + "&";

        url += "$expand=Comments($select=Id),Character";

        $.ajax({
            type: "GET", 
            url: url,
            success: function (responce) {

                if (responce && responce["@odata.count"] > 0)
                {
                    $.each(responce.value, function (index, post) {

                        var content = '<a href="/Post/Index/' + post.Id + '">';

                        if (post.Picture)
                            content += '<figure class="grid-stack-item-content"  style="background-image: url(' + post.Picture + ');">';
                        else
                            content += '<figure class="grid-stack-item-content" style="background-image: url(../Content/images/evemail.svg);">';

                        content += '<figcaption>';

                        content += '<span class="post-author">' + post.Character.Name + '</span>';

                        var date = new Date(post.AddedDate);
                        content += '<span class="post-date">' + date.getDate() + "." + (date.getMonth() + 1) + "." + date.getFullYear() + '</span>';

                        if (post.IsFixed) {
                            content += '<i class="fa fa-thumb-tack"></i>';
                        }

                        if (post.IsInternal) {
                            content += '<i class="fa fa-eye-slash"></i>';
                        }

                        if (post.Comments.length > 0) {
                            content += '<span class="commnets-preview"><i class="fa fa-comments-o"></i>:' + post.Comments.length + '</span>';
                        }

                        content += '<span class="header-preview">' + post.Header + '</span></figcaption></figure></a>';

                        _this.grid.addWidget($(content), 0, 0, post.Width, post.Height, 'yes');
                    });
                    _this.PageableData.Skip += responce["@odata.count"];
                }
            },
            error: function (responce) {
                _this.ShowError(responce.statusText);
            },
            contentType: "application/json"
        });
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

var homeIndex = null;
$().ready(function () {
    homeIndex = new HomeIndex();
    homeIndex.init();
});

