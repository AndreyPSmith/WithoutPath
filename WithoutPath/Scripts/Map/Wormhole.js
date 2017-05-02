var Wormhole = function (o) {
    var _this = this;

    _this.SpaceSystem = o;
    _this.TextHeightBase = 22.5;
    _this.first_render = true;

    this.GetName = function () {

        if (o.Name)
            return o.Name;

        o.Name = "Unknown";
        return o.Name;
    };

    this.GetPoint = function () {
        return Point = { x: _this.group.attrs.x ? _this.group.attrs.x : o.x, y: _this.group.attrs.y ? _this.group.attrs.y : o.y };
    };

    this.SetDraggable = function (draggable) {

        if (_this.group)
            _this.group.draggable(draggable);
    };

    // анимация исчезновения
    this.collapse = function () {
        o.collapsing = true;
        new Konva.Tween({
            node: _this.circle,
            duration: 0.2,
            scaleX: 0,
            scaleY: 0,
            easing: Konva.Easings.EaseOut,
            onFinish: function () {
                _this.group.destroy();
            }
        }).play();

        new Konva.Tween({
            node: _this.arc,
            duration: 0.2,
            scaleX: 0,
            scaleY: 0,
            easing: Konva.Easings.EaseOut
        }).play();

        if (_this.Shelf) {
            new Konva.Tween({
                node: _this.Shelf,
                duration: 0.2,
                scaleX: 0,
                scaleY: 0,
                easing: Konva.Easings.EaseOut
            }).play();

            new Konva.Tween({
                node: _this.TextRect,
                duration: 0.2,
                opacit: 0,
                easing: Konva.Easings.Linear
            }).play();

            new Konva.Tween({
                node: _this.Text,
                duration: 0.2,
                opacity: 0,
                easing: Konva.Easings.Linear
            }).play();

            new Konva.Tween({
                node: _this.ClassText,
                duration: 0.2,
                opacity: 0,
                easing: Konva.Easings.Linear
            }).play();

            if (_this.SpaceSystem.Pilots.length > 0) {
                new Konva.Tween({
                    node: _this.PilotsIcon,
                    duration: 0.2,
                    opacity: 0,
                    easing: Konva.Easings.Linear
                }).play();

                new Konva.Tween({
                    node: _this.PilotsLengthText,
                    duration: 0.2,
                    opacity: 0,
                    easing: Konva.Easings.Linear
                }).play();
            }

            if (_this.SpaceSystem.isWarning) {
                new Konva.Tween({
                    node: _this.WarningIcon,
                    duration: 0.2,
                    opacity: 0,
                    easing: Konva.Easings.Linear
                }).play();
            }
        }
    };

    this.render = function () {

        if (!o.Map.SystemsLevel)
            return

        /* чтобы все элементы можно было перетаскивать вместе 
           их нужно сгруппировать
        */
        _this.group = new Konva.Group({
            draggable: false
        });

        // иницилизация системы
        _this.circle = new Konva.Circle({
            x: o.x,
            y: o.y,
            radius: o.currentRadius,
            fillRadialGradientStartPoint: { x: 0, y: 0 },
            fillRadialGradientStartRadius: (_this.currentEase() * 0.0022) * o.currentRadius + o.currentRadius * 0.7,
            fillRadialGradientEndPoint: { x: 0, y: 0 },
            fillRadialGradientEndRadius: o.currentRadius,
            fillRadialGradientColorStops: [0, 'rgba(0, 0, 0, 1)', 1, 'rgba(103, 181, 191, 0.75)'],
            scaleX: 0.5,
            scaleY: 0.5
        });

        _this.group.add(_this.circle);

        // иницилизация арки
        _this.arc = new Konva.Arc({
            x: o.x,
            y: o.y,
            innerRadius: o.radius * 1.4,
            outerRadius: o.radius * 1.4,
            angle: 0,
            stroke: '#ffffff',
            strokeWidth: 2,
            visible: false
        });

        _this.group.add(_this.arc);

        _this.circle.on('mouseover', function () {
            $("#" + o.Map.Properties.CONTAINER).css('cursor', 'pointer');
            _this.group.moveToTop();
            new Konva.Tween({
                node: _this.arc,
                duration: 0.2,
                stroke: '#428bca',
                easing: Konva.Easings.Linear
            }).play();
        });
        _this.circle.on('mouseout', function () {
            $("#" + o.Map.Properties.CONTAINER).css('cursor', 'default');
            new Konva.Tween({
                node: _this.arc,
                duration: 0.2,
                stroke: '#ffffff',
                easing: Konva.Easings.Linear
            }).play();
        });

        // показываем меню
        _this.circle.on('click', function (e) {
            if (e.evt.button == 2)
                o.Map.ShowMenu(e, _this);
        });

        // добавляем обьект на слой
        o.Map.SystemsLevel.add(_this.group);

        // Анимация появления
        new Konva.Tween({
            node: _this.circle,
            duration: 2.5,
            scaleX: 1,
            scaleY: 1,
            easing: Konva.Easings.ElasticEaseOut
        }).play();

        _this.renderArc();

        // Анимация "дыхания" системы 
        var anim = new Konva.Animation(function (frame) {
            _this.circle.fillRadialGradientStartRadius((_this.currentEase() * 0.0022) * o.currentRadius + o.currentRadius * 0.7);
            _this.circle.fillRadialGradientColorStops()[3] = Math.random() < 0.005 ? 'rgba(255, 196, 0, 0.15)' : 'rgba(103, 181, 191, 0.75)';
        }, o.Map.SystemsLevel);

        anim.start();
    };

    this.renderTag = function () {

        var angle = -Math.PI / 4;

        var point = _this.GetPoint();

        var arc = {
            x: ((_this.SpaceSystem.radius * 1.4) * Math.cos(angle)),
            y: ((_this.SpaceSystem.radius * 1.4) * Math.sin(angle))
        }

        var shelf = {
            x: ((_this.SpaceSystem.radius * 2.6) * Math.cos(angle)),
            y: ((_this.SpaceSystem.radius * 2.6) * Math.sin(angle))
        }

        if (_this.TextRect)
            _this.TextRect.destroy();
        if (_this.ClassText)
            _this.ClassText.destroy();
        if (_this.TypeText)
            _this.TypeText.destroy();
        if (_this.Text)
            _this.Text.destroy();
        if (_this.PilotsIcon)
            _this.PilotsIcon.destroy();
        if (_this.PilotsLengthText)
            _this.PilotsLengthText.destroy();
        if (_this.PilotsText)
            _this.PilotsText.destroy();
        if (_this.WarningIcon)
            _this.WarningIcon.destroy();
        if (_this.WarningText)
            _this.WarningText.destroy();
        if (_this.Shelf)
            _this.Shelf.destroy();

        var Height = _this.TextHeightBase / 1.2;

        var textHeight = -Height;
        var textWidth = 0;

        // алярм!
        if (_this.SpaceSystem.isWarning) {

            _this.WarningIcon = new Konva.Text({
                opacity:  _this.first_render ? 0 : 1,
                x: shelf.x,
                y: shelf.y + textHeight,
                text: ' \uf071 ',
                fontSize: (_this.TextHeightBase / 1.3),
                fontFamily: 'FontAwesome',
                fill: '#D39D09',
                align: 'center'
            });

            _this.WarningText = new Konva.Text({
                opacity: 0,
                x: shelf.x,
                y: shelf.y + 5,
                text: "\n" + _this.SpaceSystem.Note,
                fontSize: Height / 1.2,
                lineHeight: 1.2,
                fontFamily: 'Arial',
                fill: '#ffffff',
                align: 'left',
                listening: false
            });

            // показываем информацию о системе
            _this.WarningIcon.on('mouseover', function () {
                $("#" + o.Map.Properties.CONTAINER).css('cursor', 'pointer');

                var warningTextWidth = _this.WarningText.getTextWidth();

                new Konva.Tween({
                    node: _this.WarningText,
                    duration: 0.4,
                    opacity: 1,
                    easing: Konva.Easings.EaseOut
                }).play();


                new Konva.Tween({
                    node: _this.TextRect,
                    duration: 0.2,
                    height: (Height / 1.2 * (_this.SpaceSystem.Note.split("\n").length + 2)) + 7,
                    width: warningTextWidth > textWidth ? warningTextWidth : textWidth,
                    easing: Konva.Easings.EaseOut
                }).play();

                _this.group.moveToTop();
            });

            // скрываем информацию о системе
            _this.WarningIcon.on('mouseout', function () {
                $("#" + o.Map.Properties.CONTAINER).css('cursor', 'default');

                new Konva.Tween({
                    node: _this.WarningText,
                    duration: 0.2,
                    opacity: 0,
                    easing: Konva.Easings.EaseOut
                }).play();

                new Konva.Tween({
                    node: _this.TextRect,
                    duration: 0.4,
                    height: Height,
                    width: textWidth,
                    easing: Konva.Easings.EaseOut
                }).play();

            });

            textWidth += _this.WarningIcon.getTextWidth();
        }

        var textFill = null;
        switch (_this.SpaceSystem.Type) {
            case "Black Hole":
                textFill = '#3c5f73';
                break;
            case "Cataclysmic Variable":
                textFill = '#9a692b';
                break;
            case "Magnetar":
                textFill = '#8a8f9a';
                break;
            case "Pulsar":
                textFill = '#007BD3';
                break;
            case "Red Giant":
                textFill = '#9E1716';
                break;
            case "Wolf-Rayet Star":
                textFill = '#E86600';
                break;
            default:
                textFill = '#ffffff'
        }

        // класс системы
        _this.ClassText = new Konva.Text({
            opacity: _this.first_render ? 0 : 1,
            x: shelf.x + textWidth,
            y: shelf.y + textHeight,
            text: _this.SpaceSystem.Class + " ",
            fontSize: Height,
            fontFamily: 'Arial',
            fill: textFill,
            align: 'center'
        });

        var systemDescription = "\nStatics: ";
        $.each(_this.SpaceSystem.Statics, function (index, item) {
            systemDescription += "\n" + item.Name + " - " + item.Description;
        });

        systemDescription += "\n\n" + (_this.SpaceSystem.Type ?
            _this.SpaceSystem.Type + ": \n" + _this.SpaceSystem.Effect :
            _this.SpaceSystem.Effect);

        _this.TypeText = new Konva.Text({
            opacity: 0,
            x: shelf.x,
            y: shelf.y + 5,
            text: systemDescription,
            fontSize: Height / 1.2,
            lineHeight: 1.2,
            fontFamily: 'Arial',
            fill: '#ffffff',
            align: 'left',
            listening: false
        });

        // показываем информацию о типе системы
        _this.ClassText.on('mouseover', function () {
            $("#" + o.Map.Properties.CONTAINER).css('cursor', 'pointer');

            var typeTextWidth = _this.TypeText.getTextWidth();

            new Konva.Tween({
                node: _this.TypeText,
                duration: 0.4,
                opacity: 1,
                easing: Konva.Easings.EaseOut
            }).play();

            new Konva.Tween({
                node: _this.TextRect,
                duration: 0.2,
                height: (Height / 1.2 * ((systemDescription).split("\n").length + 2)) + 7,
                width: typeTextWidth > textWidth ? typeTextWidth : textWidth,
                easing: Konva.Easings.EaseOut
            }).play();

            _this.group.moveToTop();
        });

        // скрываем информацию о типе системы
        _this.ClassText.on('mouseout', function () {
            $("#" + o.Map.Properties.CONTAINER).css('cursor', 'default');

            new Konva.Tween({
                node: _this.TypeText,
                duration: 0.2,
                opacity: 0,
                easing: Konva.Easings.EaseOut
            }).play();

            new Konva.Tween({
                node: _this.TextRect,
                duration: 0.4,
                height: Height,
                width: textWidth,
                easing: Konva.Easings.EaseOut
            }).play();

        });

        textWidth += _this.ClassText.getTextWidth();

        // название
        _this.Text = new Konva.Text({
            opacity: _this.first_render ? 0 : 1,
            x: shelf.x + textWidth,
            y: shelf.y + textHeight,
            text: " " + _this.GetName() + " ",
            fontSize: Height,
            fontFamily: 'Arial',
            fill: '#ffffff',
            align: 'center',
            listening: false
        });


        // пилоты, если они есть
        if (_this.SpaceSystem.Pilots.length > 0) {
            textWidth += _this.Text.getTextWidth();

            _this.PilotsIcon = new Konva.Text({
                opacity: _this.first_render ? 0 : 1,
                x: shelf.x + textWidth,
                y: shelf.y + textHeight + 1,
                text: '\uf2be',
                fontSize: (_this.TextHeightBase / 1.5),
                fontFamily: 'FontAwesome',
                fill: '#007BD3',
                align: 'center'
            });

            textWidth += _this.PilotsIcon.getTextWidth();

            _this.PilotsLengthText = new Konva.Text({
                opacity: _this.first_render ? 0 : 1,
                x: shelf.x + textWidth,
                y: shelf.y + textHeight,
                text: "  : " + _this.SpaceSystem.Pilots.length,
                fontSize: (_this.TextHeightBase / 1.4),
                fontFamily: 'Arial',
                fill: '#007BD3',
                align: 'center',
                listening: false
            });

            textWidth += _this.PilotsLengthText.getTextWidth();

            var pilotsList = "\n";

            for (var i = 1; i < _this.SpaceSystem.Pilots.length + 1; i++) {
                pilotsList += i + ". " + _this.SpaceSystem.Pilots[i - 1] + " \n";
            }

            _this.PilotsText = new Konva.Text({
                opacity: 0,
                x: shelf.x,
                y: shelf.y + 5,
                text: pilotsList,
                fontSize: Height / 1.2,
                fontFamily: 'Arial',
                fill: '#ffffff',
                align: 'left',
                listening: false
            });

            // показываем информацию о пилотах
            _this.PilotsIcon.on('mouseover', function () {
                $("#" + o.Map.Properties.CONTAINER).css('cursor', 'pointer');

                if (_this.SpaceSystem.Pilots.length > 0) {

                    var pilotsTextWidth = _this.PilotsText.getTextWidth();

                    new Konva.Tween({
                        node: _this.PilotsText,
                        duration: 0.4,
                        opacity: 1,
                        easing: Konva.Easings.EaseOut
                    }).play();

                    new Konva.Tween({
                        node: _this.TextRect,
                        duration: 0.2,
                        height: (Height / 1.2 * (_this.SpaceSystem.Pilots.length + 2)) + 7,
                        width: pilotsTextWidth > textWidth ? pilotsTextWidth : textWidth,
                        easing: Konva.Easings.EaseOut
                    }).play();

                    _this.group.moveToTop();
                }
            });

            // скрываем информацию о пилотах
            _this.PilotsIcon.on('mouseout', function () {
                $("#" + o.Map.Properties.CONTAINER).css('cursor', 'default');

                if (_this.SpaceSystem.Pilots.length > 0) {

                    new Konva.Tween({
                        node: _this.PilotsText,
                        duration: 0.2,
                        opacity: 0,
                        easing: Konva.Easings.EaseOut
                    }).play();

                    new Konva.Tween({
                        node: _this.TextRect,
                        duration: 0.4,
                        height: Height,
                        width: textWidth,
                        easing: Konva.Easings.EaseOut
                    }).play();
                }
            });

        }
        else {
            textWidth += _this.Text.getTextWidth();
        }

        // подложка для текста
        _this.TextRect = new Konva.Rect({
            opacity: _this.first_render ? 0 : 1,
            x: shelf.x,
            y: shelf.y + textHeight,
            stroke: 'rgba(0,0,0,0.8)',
            strokeWidth: 1,
            fill: 'rgba(0,0,0,0.8)',
            width: textWidth,
            height: -textHeight,
            listening: false
        });

        // полка
        _this.Shelf = new Konva.Line({
            opacity: _this.first_render ? 0 : 1,
            points: [arc.x, arc.y, shelf.x, shelf.y, shelf.x + textWidth + 1, shelf.y],
            stroke: '#ffffff',
            strokeWidth: 2,
            lineCap: 'round',
            lineJoin: 'round',
            listening: false
        });

        _this.group.add(_this.TextRect);
        _this.group.add(_this.ClassText);
        _this.group.add(_this.TypeText);
        _this.group.add(_this.Text);
        if (_this.SpaceSystem.Pilots.length > 0) {
            _this.group.add(_this.PilotsIcon);
            _this.group.add(_this.PilotsLengthText);
            _this.group.add(_this.PilotsText);
        }
        if (_this.SpaceSystem.isWarning) {
            _this.group.add(_this.WarningIcon);
            _this.group.add(_this.WarningText);
        }

        _this.group.add(_this.Shelf);


        if (_this.first_render) {
            _this.first_render = false;

            new Konva.Tween({
                node: _this.TextRect,
                duration: 0.2,
                opacity: 1,
                easing: Konva.Easings.Linear
            }).play();

            new Konva.Tween({
                node: _this.ClassText,
                duration: 0.2,
                opacity: 1,
                easing: Konva.Easings.Linear
            }).play();

            new Konva.Tween({
                node: _this.Text,
                duration: 0.2,
                opacity: 1,
                easing: Konva.Easings.Linear
            }).play();

            if (_this.SpaceSystem.Pilots.length > 0) {
                new Konva.Tween({
                    node: _this.PilotsIcon,
                    duration: 0.2,
                    opacity: 1,
                    easing: Konva.Easings.Linear
                }).play();

                new Konva.Tween({
                    node: _this.PilotsLengthText,
                    duration: 0.2,
                    opacity: 1,
                    easing: Konva.Easings.Linear
                }).play();
            }

            if (_this.SpaceSystem.isWarning) {
                new Konva.Tween({
                    node: _this.WarningIcon,
                    duration: 0.2,
                    opacity: 1,
                    easing: Konva.Easings.Linear
                }).play();
            }

            new Konva.Tween({
                node: _this.Shelf,
                duration: 0.2,
                opacity: 1,
                easing: Konva.Easings.Linear
            }).play();
        }

    };

    this.renderArc = function () {

        // Анимация появления арки
        if (o.Map.GetPrevious(this) == null || o.canStartRenderArc) {

            _this.arc.show();

            _this.arc.rotate(_this.SpaceSystem.incomingAngle);
            new Konva.Tween({
                node: _this.arc,
                duration: 1.5,
                angle: 360,
                easing: Konva.Easings.Linear,
                onFinish: function () {
                    o.arcRenderFinish = true;
                    _this.renderTag();
                }
            }).play();
        }
    };

    this.moveToPoint = function (Point, animate) {

        if (animate) {
            new Konva.Tween({
                node: _this.group,
                duration: 1,
                x: Point.x,
                y: Point.y,
                easing: Konva.Easings.EaseOut
            }).play();
        }
        else {
            _this.group.setX(Point.x);
            _this.group.setY(Point.y);
        }


        _this.SpaceSystem.x = Point.x;
        _this.SpaceSystem.y = Point.y;
    };

    this.currentEase = function () {

        o._currentEase += o._easeIsGrow ? +0.5 : -0.5;

        if (o._easeIsGrow && o._currentEase > 100)
            o._easeIsGrow = false;

        if (!o._easeIsGrow && o._currentEase <= 1)
            o._easeIsGrow = true;

        return o._currentEase;
    };
};

