function WithoutPath() {
    var _this = this;

    _this.Properties =
    {
        UPDATE_INTERVAL: 10000,
        SYSTEM_RADIUS: 15,
        ARC_X: -250,
        ARC_Y: -50,
        ARC_R: [450, 600],
        HOME_X: 55,
        HOME_Y: 80,
        SCALE: 1,
        HEIGHT: 700,
        CONTAINER: 'Layout',
        MENU: 'contextMenu',
        ApiSpaceSystems: "/api/SpaceSystems",
        ApiSystemTypes: "/api/SystemTypes",
        dragging: false,
        lastX: 0,
        lastY: 0,
        offsetX: 0,
        offsetY: 0
    }

    _this.LinkStatus = {
        None: 0,
        HalfMass: 1,
        CriticalMass: 2,
        CriticalTime: 4
    };

    _this.Links = [];
    _this.Levels = [];
    _this.Arcs = [];
    _this.Systems = [];

    _this.SystemTypes = [];

    _this.CriticalError = false;
    _this.IsMenuOpen = false;

    _this.Hub = null;

    this.init = function () {

        $(document).on("contextmenu", "#Content", function (e) {
            e.preventDefault();
            e.stopPropagation();
        });

        $(window).resize(function () {
            _this.Stage.setWidth($("div.map-wrapper").width());
        });

        $(window).mouseup(function () {
            _this.Properties.dragging = false;
            $("#" + _this.Properties.CONTAINER).css('cursor', 'default');
        });

        $(document).on("mouseleave", "#" + _this.Properties.MENU, function () {
            $("#" + _this.Properties.MENU).hide();
        });

        $(document).on("click", "#Offset", function () {

            new Konva.Tween({
                node: _this.Stage,
                duration: 0.5,
                offsetX: 0,
                offsetY: 0,
                easing: Konva.Easings.Linear,
                onFinish: function () {
                    _this.Properties.lastX = 0;
                    _this.Properties.lastY = 0;
                    _this.Properties.offsetX = 0;
                    _this.Properties.offsetY = 0;

                    $("#Offes_X").html(0);
                    $("#Offes_Y").html(0);
                }
            }).play();
        });

        _this.initMap();
        _this.initHub();
    };

    this.initMap = function () {

        _this.Stage = new Konva.Stage({
            container: _this.Properties.CONTAINER,  // индификатор div контейнера
            width: $("div.map-wrapper").width(),
            height: _this.Properties.HEIGHT + 100
        });

        // Тут рисуем уровни
        _this.LevelsLayer = new Konva.Layer();
        _this.Stage.add(_this.LevelsLayer);

        _this.LayoutLayer = new Konva.Layer();

        // подложка для скролла
        _this.LayoutRect = new Konva.Rect({
            opacity: 0,
            x: 0,
            y:0,
            stroke: 'rgba(0,0,0,0.0)',
            strokeWidth: 1,
            fill: 'rgba(0,0,0,0.0)',
            width: 9999999,
            height: 9999999
        });

        _this.LayoutLayer.add(_this.LayoutRect);
        _this.Stage.add(_this.LayoutLayer);

        // иницифлизация слоя ссылок
        _this.LinksLevel = new Konva.Layer();
        _this.Stage.add(_this.LinksLevel);

        // иницифлизация слоя с системами
        _this.SystemsLevel = new Konva.Layer();
        _this.Stage.add(_this.SystemsLevel);

        _this.LayoutLayer.on('mousedown', function (e) {
            if (e.evt.button == 0) {

                $("#" + _this.Properties.CONTAINER).css('cursor', 'move');

                _this.Properties.dragging = true;
                _this.Properties.lastX = e.evt.offsetX;
                _this.Properties.lastY = e.evt.offsetY;
            }
        });

        _this.LayoutLayer.on('mouseleave', function (e) {
            _this.Properties.dragging = false;
            $("#" + _this.Properties.CONTAINER).css('cursor', 'default');
        });

        _this.LayoutLayer.on('mousemove', function (e) {
            if (_this.Properties.dragging) {

                var deltaX = e.evt.offsetX - _this.Properties.lastX;
                var deltaY = e.evt.offsetY - _this.Properties.lastY;

                _this.Properties.offsetX += deltaX;
                _this.Properties.offsetY += deltaY;

                _this.Properties.lastX = e.evt.offsetX;
                _this.Properties.lastY = e.evt.offsetY;

                $("#Offes_X").html(Math.round(_this.Properties.offsetX * -1));
                $("#Offes_Y").html(Math.round(_this.Properties.offsetY * -1));

                _this.Stage.offsetX(-_this.Properties.offsetX);
                _this.Stage.offsetY(-_this.Properties.offsetY);

            }
        });

        // перерисовка ссылок
        var LinksAnim = new Konva.Animation(function (frame) {

            for (i = 0, len = _this.Links.length; i < len; i++) {
                var l = _this.Links[i];
                l.render();
                if (l.destroyed) {
                    _this.Links.splice(i, 1);
                    len--;
                    i--;
                }
            }

        }, _this.LinksLevel);

        LinksAnim.start();
        // перерисовка уровней
        var LayerAnim = new Konva.Animation(function (frame) {
        }, _this.LevelsLayer);

        LayerAnim.start();
        // рисуем уровни 
        _this.DrawLevels();
    };

    this.initHub = function () {

        // Ссылка на автоматически-сгенерированный прокси хаба
        _this.Hub = $.connection.mapHub;

     
        // Бродкаст от хаба 
        _this.Hub.client.addMessage = function (Systems) {
            _this.UpdateData(Systems);
        };

        // Функция, вызываемая при подключении для нового пользователя 
        _this.Hub.client.onConnected = function (Systems) {
            _this.UpdateData(Systems);
        }

        $.ajax({
            type: "GET",
            url: _this.Properties.ApiSystemTypes,
            success: function (responce) {
                _this.SystemTypes = responce.value;

                // Открываем соединение 
                $.connection.hub.start().done(function () {
                    _this.Hub.server.connect();
                });

            },
            error: function (responce) {

                _this.ShowError(responce.statusText);
                _this.CriticalError = true;

            },
            contentType: "application/json"
        });

    };

    this.DrawLevels = function () {

        _this.SortSystems();

        for (var i = 0; i < _this.Arcs.length; i++) {
            _this.Arcs[i].destroy();
        }

        _this.Arcs = [];

        for (i = 1; i <= _this.Levels.length / 2; i += 0.5) {

            var Arc = new Konva.Arc({
                x: _this.Properties.ARC_X * i,
                y: _this.Properties.ARC_Y * i,
                innerRadius: _this.Properties.ARC_R[_this.Properties.SCALE] * i,
                outerRadius: _this.Properties.ARC_R[_this.Properties.SCALE] * i,
                stroke: 'rgba(255, 255, 255, 0.3)',
                strokeWidth: 2,
                angle: 60,
                rotation: 5
            });
            _this.Arcs.push(Arc);
            _this.LevelsLayer.add(Arc);
        }
    }

    this.SortSystems = function () {

        for (i = 0, len = _this.Systems.length; i < len; i++) {
            var s = _this.Systems[i];
            s.LinkedToHome = false;
            if (s.collapsing) {
                _this.Systems.splice(i, 1);
                len--;
                i--;
            }
        }

        _this.buildHierarchy();

        _this.Properties.SCALE = _this.Levels.length < 7 ? 1 : 0;

        for (lvl = 0; lvl < _this.Levels.length; lvl++) {

            var items_on_lvl = _this.Levels[lvl].length;

            for (i = 0; i < _this.Levels[lvl].length; i++) {

                _this.Levels[lvl][i].LinkedToHome = true;
                _this.Levels[lvl][i].IsInHeap = false;
                _this.Levels[lvl][i].SetDraggable(false);

                if (_this.Levels[lvl][i].isHome) {
                    var POINT = {
                        x: _this.Properties.HOME_X,
                        y: _this.Properties.HOME_Y
                    }

                    _this.Levels[lvl][i].moveToPoint(POINT, _this.Levels[lvl][i].IsMovedToPoint);
                }
                else {
                    var cur_lvl = lvl + 1;

                    var x = (_this.Properties.ARC_X * (cur_lvl / 2));
                    var y = (_this.Properties.ARC_Y * (cur_lvl / 2));
                    var r = (_this.Properties.ARC_R[_this.Properties.SCALE] * (cur_lvl / 2));

                    var dy = -y;
                    var dx = Math.sqrt(r * r - dy * dy);

                    var min_angle = Math.atan2(-y, dx) - 0.015;

                    if (_this.Properties.SCALE == 1 && cur_lvl < 4)
                        min_angle = Math.atan2(-y, dx) - 0.05;

                    if (_this.Properties.SCALE == 0 && cur_lvl < 3)
                        min_angle = Math.atan2(-y, dx) - 0.05;

                    var max_angle = (Math.PI / 4) + 0.19;

                    if (cur_lvl > (_this.Properties.SCALE == 0 ? 4 : 3)) {

                        dy = (_this.Properties.HEIGHT - y);
                        dx = Math.sqrt(r * r - dy * dy);

                        max_angle = Math.atan2(dy, dx);
                    }

                    var angle = max_angle - ((max_angle - min_angle) * ((100 / (items_on_lvl + 1) * (items_on_lvl - i)) / 100));

                    var POINT = {
                        x: (_this.Properties.ARC_X * (cur_lvl / 2)) + ((_this.Properties.ARC_R[_this.Properties.SCALE] * (cur_lvl / 2)) * Math.cos(angle)),
                        y: (_this.Properties.ARC_Y * (cur_lvl / 2)) + ((_this.Properties.ARC_R[_this.Properties.SCALE] * (cur_lvl / 2)) * Math.sin(angle))
                    }
                    _this.Levels[lvl][i].moveToPoint(POINT, _this.Levels[lvl][i].IsMovedToPoint);
                }
            }
        }

        var HeapOffset = (_this.Properties.ARC_R[_this.Properties.SCALE] * _this.Levels.length / 2) / 2 + 250

        if (HeapOffset != _this.HeapOffset) {
            var difference = _this.HeapOffset - HeapOffset;
            _this.HeapOffset = HeapOffset;

            for (i = 0, len = _this.Systems.length; i < len; i++) {
                var s = _this.Systems[i];
                if (s.IsInHeap) {
                    var POINT = s.Point();
                    POINT.x -= difference;
                    s.moveToPoint(POINT, s.IsMovedToPoint);
                }
            }
        }

        for (i = 0, len = _this.Systems.length; i < len; i++) {
            var s = _this.Systems[i];
            if (!s.LinkedToHome && !s.IsInHeap) {

                var parent = _this.GetPrevious(s);
                if (parent == null || (parent != null && parent.collapsing)) {
                    var Offset_X_min = _this.HeapOffset + (_this.Properties.SYSTEM_RADIUS * 3);
                    var Offset_X_max = _this.HeapOffset + (_this.Properties.SYSTEM_RADIUS * 15);

                    var min_offset = _this.Properties.SYSTEM_RADIUS * 5;

                    for (var j = 0; j < 100; j++) {

                        switch (j) {
                            case 90:
                                Offset_X_max += Offset_X_max;
                        }

                        var POINT = {
                            x: _this.getRandomInt(Offset_X_min, Offset_X_max),
                            y: _this.getRandomInt((_this.Properties.SYSTEM_RADIUS * 5), _this.Properties.HEIGHT - (_this.Properties.SYSTEM_RADIUS * 5))
                        }

                        var IsIntercept = false;

                        for (k = 0, len = _this.Systems.length; k < len; k++) {

                            if (_this.Systems[k].IsInHeap) {

                                var point = _this.Systems[k].Point();

                                var dx = point.x - POINT.x,
                                    dy = point.y - POINT.y;

                                if (Math.sqrt(dx * dx + dy * dy) < min_offset) {
                                    IsIntercept = true;
                                    break;
                                }
                            }
                        }

                        if (!IsIntercept || j == 99) {
                            s.moveToPoint(POINT, s.IsMovedToPoint);
                            s.SetDraggable(true);
                            s.IsInHeap = true;
                            break;
                        }
                    }

                    if ((parent == null || parent.collapsing) && !s.arcRenderFinish) {
                        s.canStartRenderArc = true;
                        s.renderArc();
                    }
                }
            }
        }

        for (i = 0, len = _this.Systems.length; i < len; i++) {
            var s = _this.Systems[i];
            if (!s.LinkedToHome && !s.IsInHeap) {

                var parent = _this.GetPrevious(s);

                if (parent != null && !parent.collapsing) {
                    var parent_position = parent.Point();
                    var offset = (_this.Properties.SYSTEM_RADIUS * 10);
                    var min_offset = _this.Properties.SYSTEM_RADIUS * 5;

                    var min_y = (_this.Properties.SYSTEM_RADIUS * 5);
                    var max_y = _this.Properties.HEIGHT - (_this.Properties.SYSTEM_RADIUS * 5);

                    for (var j = 0; j < 100; j++) {

                        switch (j) {
                            case 90:
                                offset += offset;
                        }

                        var POINT = {
                            x: parent_position.x + _this.getRandomInt(-offset, offset),
                            y: parent_position.y + _this.getRandomInt(-offset, offset)
                        }

                        var IsIntercept = false;

                        if (POINT.x > _this.HeapOffset && POINT.y > min_y && POINT.y < max_y) {
                            for (k = 0, len = _this.Systems.length; k < len; k++) {
                                if (_this.Systems[k].IsInHeap) {
                                    var point = _this.Systems[k].Point();

                                    var dx = point.x - POINT.x,
                                        dy = point.y - POINT.y;

                                    if (Math.sqrt(dx * dx + dy * dy) < min_offset) {
                                        IsIntercept = true;
                                        break;
                                    }
                                }
                            }

                            if (!IsIntercept || j == 99) {
                                s.moveToPoint(POINT, s.IsMovedToPoint);
                                s.SetDraggable(true);
                                s.IsInHeap = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    this.GetPrevious = function (system) {
        for (var i = 0; i < _this.Links.length; i++) {

            var link = _this.Links[i];

            if (!_this.Links[i].collapsing && system.GetName() == _this.Links[i].To.GetName())
                return _this.Links[i].From;
        }
        return null;
    }

    this.getRandomInt = function (min, max) {
        return Math.floor(Math.random() * (max - min)) + min;
    }

    this.buildHierarchy = function () {

        _this.Levels = [];

        var root = null;

        for (var i = 0; i < _this.Systems.length; i++) {
            if (_this.Systems[i].isHome) {
                root = { value: _this.Systems[i], children: [] , parent : null};
                break;
            }
        }

        if(root != null)
        {
            var findChildren = function (current, parent) {
                if (current && current.children) {
                    for (var j = 0; j < _this.Links.length; j++) {

                        var item = _this.Links[j];

                        if (!item.collapsing) {

                            var child = null;

                            if (current.value.Id == item.To.Id && (!parent || parent.Id != item.From.Id)) {
                                child = item.From;
                            }
                            else if (current.value.Id == item.From.Id && (!parent || parent.Id != item.To.Id)) {
                                child = item.To;
                            }

                            if(child != null)
                            {
                                if (!IsChildExis(child, current))
                                current.children.push({ value: child, children: [], parent: current });
                            }
                        }
                    }

                   
                    $.each(current.children, function (index, item) {

                        findChildren(item, current.value);

                    });
                }
            };

            var IsChildExis = function (current, parent) {
                
                if (!parent)
                    return false;
                else if (current.Id == parent.value.Id)
                    return true;
                else
                    return IsChildExis(current, parent.parent)
                
            };

            findChildren(root, null);

           
            var system_on_levels = [];
            var buildlevels = function (item, lvl) {

                while (!_this.Levels[lvl]) {
                    _this.Levels.push([]);
                }

                if (item) {

                    var exist = false;
                    $.each(system_on_levels, function (index, system) {
                        if (system == item.value.Name)
                            exist = true;
                    });

                    if (!exist) {
                        _this.Levels[lvl].push(item.value);
                        system_on_levels.push(item.value.Name);

                        if (item.children) {
                            $.each(item.children, function (index, value) {
                                buildlevels(value, (lvl + 1));
                            });
                        }
                    }
                }
            }

            buildlevels(root, 0);
        }
    }

    this.ShowMenu = function (e, system) {

        _this.IsMenuOpen = true;
      
        var Events = [];

        if (!system.SpaceSystem.isWormhole)
            Events.push({
                Type: "dest",
                Name: "Set Destination",
                Icon: 'fa-map-marker'
            });

        Events.push({
            Type: "note",
            Name: "Set Note",
            Icon: 'fa-file-text-o'
        });

        if (!system.SpaceSystem.isHome)
            Events.push({
                Type: "delete",
                Name: "Remove",
                Icon: 'fa-times'
            });

        var menu = $("#" + _this.Properties.MENU);
        menu.find("li").remove();
        menu.css({ left: e.evt.offsetX - 10, top: e.evt.offsetY - 10 });

        for (var i = 0; i < Events.length; i++) {
            var event = Events[i];
            $('<li type="' + event.Type + '"><i class="fa ' + event.Icon + '"></i><span>' + event.Name + '</span></li>').appendTo(menu);

            menu.find("li").last().click(function () {

                var type = $(this).attr("Type");

                if (type == "dest") {
                    _this.Hub.server.setDestination(system.SpaceSystem.EveID).done(function (result) {
                        if (result.IsError)
                            _this.ShowError(result.Message);
                    });
                }

                if (type == "note") {
                    _this.ShowEditSystemNoteForm(system.SpaceSystem);
                }

                if (type == "delete") {
                    _this.Hub.server.removeSystem(system.SpaceSystem.Id).done(function (result) {
                        if (result.IsError)
                            _this.ShowError(result.Message);
                    });
                }

                $("#" + _this.Properties.MENU).hide();
                _this.IsMenuOpen = false;
            });
        }

        $("#" + _this.Properties.MENU).show();
    }

    this.ShowLinkMenu = function (e, link) {

        _this.IsMenuOpen = true;
      
        var Events = [{
            Type: "weight",
            Name: "Set Weight",
            Icon: 'fa-cube'
        }, {
            Type: "time",
            Name: "Set Time",
            Icon: 'fa-clock-o'
        }, {
            Type: "delete",
            Name: "Remove",
            Icon: 'fa-times'
        }];

        var menu = $("#" + _this.Properties.MENU);
        menu.find("li").remove();
        menu.css({ left: e.evt.offsetX - 10, top: e.evt.offsetY - 10 });

        for (var i = 0; i < Events.length; i++) {
            var event = Events[i];
            $('<li type="' + event.Type + '"><i class="fa ' + event.Icon + '"></i><span>' + event.Name + '</span></li>').appendTo(menu);

            menu.find("li").last().click(function () {

                var type = $(this).attr("Type");

                if (type == "weight") {
                    _this.ShowEditLinkMassForm(link);
                }

                if (type == "time") {
                    _this.ShowEditLinkTimeForm(link);
                }

                if (type == "delete") {
                    _this.Hub.server.removeLink(link.From.Name, link.To.Name).done(function (result) {
                        if (result.IsError)
                            _this.ShowError(result.Message);
                    });

                }

                $("#" + _this.Properties.MENU).hide();
                _this.IsMenuOpen = false;
            });
        }

        $("#" + _this.Properties.MENU).show();
    }

    this.findSystem = function (Name) {

        for (i = 0, len = _this.Systems.length; i < len; i++) {
            if (_this.Systems[i].Name == Name)
                return _this.Systems[i];
        }

        return null;
    }

    this.findLink = function (Id) {

        for (i = 0, len = _this.Links.length; i < len; i++) {
            if (_this.Links[i].Id == Id)
                return _this.Links[i];
        }

        return null;
    }

    this.UpdateData = function (data) {

        var new_systems = [];
        var systems_collapse = [];
        var links_collapse = [];

        $.each(_this.Systems, function (index, item) {
            systems_collapse.push(item.Name);
        });

        $.each(_this.Links, function (index, item) {
            links_collapse.push(item.Id);
        });

        $.each(data.Systems, function (index, item) {

            var current_index = systems_collapse.indexOf(item.Name);
            if (current_index != -1)
                systems_collapse.splice(current_index, 1);

            var Pilots = [];

            if (item.Characters)
                $.each(item.Characters, function (index, pilot) {
                    Pilots.push(pilot.Name + (pilot.Ship ? " - " + pilot.Ship : ""));
                });

            var system = _this.findSystem(item.Name);
            if (system != null) {
                if (!system.Pilots.equals(Pilots) ||
                    system.isWarning != item.Warning ||
                    system.Note != item.Note) {
                    system.Pilots = Pilots;
                    system.isWarning = item.Warning;
                    system.Note = item.Note;
                    system.renderTag();
                }
            }
            else {
                var system = new SpaceSystem({
                    Map: _this,
                    Id: item.Id,
                    EveID: item.EveID,
                    radius: _this.Properties.SYSTEM_RADIUS * (item.IsHome ? 1.5 : 1),
                    isHome: item.IsHome,
                    isWormhole: item.IsWormhole,
                    Name: item.Name,
                    Class: (item.SystemType != null) ? item.SystemType.Class : null,
                    Type: (item.SystemType != null) ? item.SystemType.Type : null,
                    Effect: (item.SystemType != null) ? item.SystemType.Description : null,
                    Security: item.Security,
                    Pilots: Pilots,
                    isWarning: item.Warning,
                    Note: item.Note,
                    Statics: item.Statics
                });

                _this.Systems.push(system);
                new_systems.push({ source: item, system: system });
            }
        });

        $.each(data.Links, function (index, item) {

            var current_index = links_collapse.indexOf(item.Id);
            if (current_index != -1)
                links_collapse.splice(current_index, 1);

            var link = _this.findLink(item.Id);
            if (link != null) {
                link.status = item.Status;
                link.updateStatus();
            }
            else {
                var From = _this.findSystem(item.From);
                var To = _this.findSystem(item.To);
                if (From != null && To != null)
                    _this.Links.push(new Link(item.Id, From, To, item.Status, _this));
            }
        });

        $.each(systems_collapse, function (index, item) {
            var system = _this.findSystem(item);
            if (system != null)
                system.collapse();
        });

        $.each(links_collapse, function (index, Id) {
            var link = _this.findLink(Id);
            if (link != null)
                link.collapse();
        });

        $.each(new_systems, function (index, item) {
            item.system.render();
        });

        _this.DrawLevels();

        _this.IsMoveAnim = true;
    }

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

        text += "</div><div class='modal-footer'></div></div>";

        $(".modal-backdrop").remove();
        var popupWrapper = $("#PopupWrapper");
        popupWrapper.empty();
        popupWrapper.html(text);
        var popup = $(".modal", popupWrapper);
        $(".modal", popupWrapper).modal();
    };

    this.ShowEditLinkTimeForm = function (link) {
        var text = "";
        text += "<div class='modal fade' tabindex='-1' role='dialog' aria-hidden='true'>";
        text += " <div class='modal-header'>";
        text += "      <button type='button' class='close' data-dismiss='modal' aria-hidden='true'><i class='fa fa-times'></i></button>";
        text += "  </div>";
        text += "<div class='modal-body'>";
        text += "<legend><i class='fa fa-clock-o'></i> Время</legend>";
        text += "<ul class='link-form-ul'>";

        text += "<li><label class='block' for='TimeStart'><i class='fa fa-hourglass-start'></i></label> <input type='radio' id='TimeStart' name='time' " +
                ((_this.LinkStatus.CriticalTime & link.status) ? "" : "checked='checked'") + "> <label class='label-for' for='TimeStart'> Normal</label></li>";
        text += "<li><label class='block' for='TimeEnd'><i class='fa fa-hourglass-end'></i></label> <input type='radio' id='TimeEnd' name='time' " +
                ((_this.LinkStatus.CriticalTime & link.status) ? "checked='checked'" : "") + "> <label class='label-for' for='TimeEnd'> End of life</label></li>";

        text += "</ul></div><div class='modal-footer'>";
        text += "<button id='SetLinkTimeStatus' class='btn btn-primary btn-login-popap'>Ok</button></div></div>";

        $(".modal-backdrop").remove();
        var popupWrapper = $("#PopupWrapper");
        popupWrapper.empty();
        popupWrapper.html(text);

        $("#SetLinkTimeStatus").click(function () {

            $('.modal').modal('hide');

            var id = $(".link-form-ul").find("input:checked").attr('id');

            link.status = link.status & (~_this.LinkStatus.CriticalTime);

            if (id == "TimeEnd") {
                link.status = link.status | _this.LinkStatus.CriticalTime;
            }

            _this.Hub.server.updateLink(link.Id, link.status).done(function (result) {
                if (result.IsError)
                    _this.ShowError(result.Message);
            });
        });

        $(".modal", popupWrapper).modal();
    };

    this.ShowEditLinkMassForm = function (link) {
        var text = "";
        text += "<div class='modal fade' tabindex='-1' role='dialog' aria-hidden='true'>";
        text += " <div class='modal-header'>";
        text += "      <button type='button' class='close' data-dismiss='modal' aria-hidden='true'><i class='fa fa-times'></i></button>";
        text += "  </div>";
        text += "<div class='modal-body'>";
        text += "<legend><i class='fa fa-cube'></i> Масса</legend>";
        text += "<ul class='link-form-ul'>";

        text += "<li><label class='block' for='MassFull'><i class='fa fa-circle'></i></label> <input type='radio' id='MassFull' name='time' ";
        if (!(_this.LinkStatus.HalfMass & link.status) && !(_this.LinkStatus.CriticalMass & link.status)) text += "checked='checked'";
        text += "> <label class='label-for' for='MassFull'> Not disrupted</label></li>"

        text += "<li><label class='block' for='MassHalf'><i class='fa fa-adjust'></i></label> <input type='radio' id='MassHalf' name='time' ";
        if (_this.LinkStatus.HalfMass & link.status) text += "checked='checked'";
        text += "> <label class='label-for' for='MassHalf'> Significant disrupted</label></li>"

        text += "<li><label class='block' for='MassFourth'><i class='fa fa-circle-o'></i></label> <input type='radio' id='MassFourth' name='time' ";
        if (_this.LinkStatus.CriticalMass & link.status) text += "checked='checked'";
        text += "> <label class='label-for' for='MassFourth'> Verge of collapse</label></li>";

        text += "</ul></div><div class='modal-footer'>";
        text += "<button id='SetLinkMassStatus' class='btn btn-primary btn-login-popap'>Ok</button></div></div>";

        $(".modal-backdrop").remove();
        var popupWrapper = $("#PopupWrapper");
        popupWrapper.empty();
        popupWrapper.html(text);

        $("#SetLinkMassStatus").click(function () {

            $('.modal').modal('hide');

            var id = $(".link-form-ul").find("input:checked").attr('id');

            link.status = link.status & (~_this.LinkStatus.HalfMass);
            link.status = link.status & (~_this.LinkStatus.CriticalMass);

            if (id == "MassHalf") {
                link.status = link.status | _this.LinkStatus.HalfMass;
            }

            if (id == "MassFourth") {
                link.status = link.status | _this.LinkStatus.CriticalMass;
            }

            _this.Hub.server.updateLink(link.Id, link.status).done(function (result) {
                if (result.IsError)
                    _this.ShowError(result.Message);
            });
        });

        $(".modal", popupWrapper).modal();
    };

    this.ShowEditSystemNoteForm = function (system) {
        var text = "";
        text += "<div class='modal fade' tabindex='-1' role='dialog' aria-hidden='true'>";
        text += " <div class='modal-header'>";
        text += "      <button type='button' class='close' data-dismiss='modal' aria-hidden='true'><i class='fa fa-times'></i></button>";
        text += "  </div>";
        text += "<div class='modal-body'>";
        text += "<legend><i class='fa fa-info-Arc'></i> Информация</legend>";

        text += "<label class='inline' for='CheckWarning'><i class='fa fa-exclamation-triangle system-note'></i></label> <input type='checkbox' id='CheckWarning'";
        if (system.isWarning) text += "checked='checked'";
        text += "> <label class='label-for' for='CheckWarning'> Is Warning?</label></li>";

        text += "<textarea id='SystemNote' class='note-textarea' rows='3'>" + (system.Note ? system.Note : "") + "</textarea>";

        text += "</div><div class='modal-footer'>";
        text += "<button id='SetSystemNote' class='btn btn-primary btn-login-popap'>Ok</button></div></div>";

        $(".modal-backdrop").remove();
        var popupWrapper = $("#PopupWrapper");
        popupWrapper.empty();
        popupWrapper.html(text);

        $("#SetSystemNote").click(function () {

            var note = $('#SystemNote').val();
            var isWarning = $("#CheckWarning").is(':checked');

            $('.modal').modal('hide');

            _this.Hub.server.updateSystem(system.Id, note, isWarning).done(function (result) {
                if (result.IsError)
                    _this.ShowError(result.Message);
            });
        });

        $(".modal", popupWrapper).modal();
    };
}

var withoutPath = null;

$().ready(function () {

    withoutPath = new WithoutPath();
    withoutPath.init();

});

