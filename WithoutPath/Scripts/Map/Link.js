/**
 * Link
 */
var Link = function (Id, from, to, status, map) {

    var _this = this;

    this.Map = map;
    this.Id = Id;
    this.From = from;
    this.To = to;
    this.progress = 0;
    this.Speed = 5;
    this.distanceCurrent = 0;
    this.lineWidth = 2;
    this.status = status; // битовая маска состояния

    this.PointFrom = { x: 0, y: 0 };
    this.PointTo = { x: 0, y: 0 };

    this.PointProgress = 0;
    this.PointProgressLimit = 300 + Math.floor(Math.random() * (200 - 20)) + 20;

    this.destroyed = false;
    this.collapsing = false;
    this.IsDone = false;

    this.updateStatus = function () {

        if (_this.Map.LinkStatus.CriticalTime & _this.status) {
            _this.line.dash([33, 10]);
        }
        else
            _this.line.dash([]);

        if (_this.Map.LinkStatus.HalfMass & _this.status) {
            _this.line.stroke('#D39D09');
        } else if (_this.Map.LinkStatus.CriticalMass & _this.status) {
            _this.line.stroke('#9E1716');
        }
        else
            _this.line.stroke('#ffffff');

    };

    this.Point = new Konva.Circle({
        radius: 3,
        fill: '#ffffff',
        visible: false
    });

    this.Point.perfectDrawEnabled(false);

    this.line = new Konva.Line({
        stroke: '#ffffff',
        strokeWidth: _this.lineWidth,
        lineCap: 'round',
        lineJoin: 'round'
    });

    _this.updateStatus();

    this.line_hover = new Konva.Line({
        stroke: "transparent",
        strokeWidth: _this.lineWidth * 5,
        lineCap: 'round',
        lineJoin: 'round'
    });

    this.line_hover.on('mouseover', function () {
        $("#" + _this.Map.Properties.CONTAINER).css('cursor', 'pointer');
        new Konva.Tween({
            node: _this.line,
            duration: 0.2,
            stroke: '#428bca',
            easing: Konva.Easings.Linear
        }).play();
    });

    this.line_hover.on('mouseout', function () {
        $("#" + _this.Map.Properties.CONTAINER).css('cursor', 'default');

        var stroke = '#ffffff';

        if (_this.Map.LinkStatus.HalfMass & _this.status) {
            var stroke = '#D39D09';
        } else if (_this.Map.LinkStatus.CriticalMass & _this.status) {
            var stroke = '#9E1716';
        }

        new Konva.Tween({
            node: _this.line,
            duration: 0.2,
            stroke: stroke,
            easing: Konva.Easings.Linear
        }).play();
    });

    this.line_hover.on('click', function (e) {
        if (e.evt.button == 2)
            _this.Map.ShowLinkMenu(e, _this);
    });

    this.line.perfectDrawEnabled(false);

    this.Map.LinksLevel.add(this.line);
    this.Map.LinksLevel.add(this.Point);
    this.Map.LinksLevel.add(this.line_hover);

    this.collapse = function () {
        this.collapsing = true;
        this.Speed = 10;
    };


    this.render = function () {

        if (!_this.From.arcRenderFinish ||
            _this.destroyed) {
            return;
        }
        else if (_this.collapsing && _this.Progress <= 0) {
            _this.destroyed = true;
            _this.line.destroy();
            _this.line_hover.destroy();
            _this.Point.destroy();
            return;
        }
        else if ((_this.From.collapsing || _this.To.collapsing) && !_this.collapsing) {
            _this.collapsing = true;
            _this.Speed = 10;
        }

        _this.PointFrom = _this.From.GetPoint();
        _this.PointTo = _this.To.GetPoint();

        var angle = _this.angleTo(_this.PointFrom, _this.PointTo);
        var distance = _this.distanceTo(_this.PointFrom, _this.PointTo);

        if (!_this.IsDone && _this.Progress >= 100) {
            _this.IsDone = true;
            _this.To.incomingAngle = angle * 180 / Math.PI;
            _this.To.canStartRenderArc = true;
            _this.To.renderArc();
        }

        var distanceTotal = distance - ((_this.From.radius * 1.4) + (_this.To.radius * 1.4));
        _this.distanceCurrent += _this.Speed * (_this.collapsing ? -1 : 1);
        _this.distanceCurrent = _this.distanceCurrent > distanceTotal ? distanceTotal : _this.distanceCurrent;
        _this.Progress = distanceTotal <= 0 ? 0 : Math.round(_this.distanceCurrent * 100 / distanceTotal);

        var shiftFrom, shiftTo;

        if (_this.collapsing && _this.From.collapsing) {
            if (_this.IsDone) {
                shiftFrom = (_this.From.radius * 1.4) + distanceTotal - _this.distanceCurrent;
                shiftTo = (_this.To.radius * 1.4);
            }
            else {
                _this.distanceCurrent += _this.Speed * 2;
                _this.distanceCurrent = _this.distanceCurrent > distanceTotal ? distanceTotal : _this.distanceCurrent;
                _this.Progress = distanceTotal <= 0 ? 0 : Math.round(_this.distanceCurrent * 100 / distanceTotal);

                if (_this.Progress == 100 && !_this.To.canStartRenderArc) {
                    _this.To.incomingAngle = angle * 180 / Math.PI;
                    _this.To.canStartRenderArc = true;
                    _this.To.renderArc();
                }

                if (!_this.distanceCurrent2)
                    _this.distanceCurrent2 = 0;

                _this.distanceCurrent2 += _this.Speed;
                _this.distanceCurrent2 = _this.distanceCurrent2 > distanceTotal ? distanceTotal : _this.distanceCurrent2;

                _this.Progress = Math.round(_this.distanceCurrent2 * 100 / distanceTotal);

                if (_this.Progress == 100)
                    _this.Progress = 0;

                shiftFrom = (_this.From.radius * 1.4) + _this.distanceCurrent2;
                shiftTo = (_this.To.radius * 1.4) + distanceTotal - _this.distanceCurrent;
            }
        }
        else {
            shiftFrom = (_this.From.radius * 1.4);
            shiftTo = _this.IsDone && !_this.collapsing ? (_this.To.radius * 1.4) : (_this.To.radius * 1.4) + distanceTotal - _this.distanceCurrent;
        }

        _this.PointFrom = {
            x: _this.PointFrom.x - (shiftFrom * Math.cos(angle)),
            y: _this.PointFrom.y - (shiftFrom * Math.sin(angle))
        };

        _this.PointTo = {
            x: _this.PointTo.x + (shiftTo * Math.cos(angle)),
            y: _this.PointTo.y + (shiftTo * Math.sin(angle))
        };

        _this.line.points([_this.PointFrom.x, _this.PointFrom.y, _this.PointTo.x, _this.PointTo.y]);
        _this.line_hover.points([_this.PointFrom.x, _this.PointFrom.y, _this.PointTo.x, _this.PointTo.y]);

        if (_this.IsDone && !_this.collapsing) {
            if (_this.PointProgress <= 100) {
                var pointPosition = (_this.From.radius * 1.4) + ((distanceTotal - 2) * _this.PointProgress / 100);

                _this.Point.show();

                _this.Point.setX(_this.From.GetPoint().x - (pointPosition * Math.cos(angle)));
                _this.Point.setY(_this.From.GetPoint().y - (pointPosition * Math.sin(angle)));

            }
            else {
                _this.Point.hide();
            }

            _this.PointProgress += 1;

            if (_this.PointProgress > _this.PointProgressLimit) {
                _this.PointProgress = 0;
                _this.PointProgressLimit = 300 + Math.floor(Math.random() * (200 - 20)) + 20;
            }
        }
        else {
            _this.Point.hide();
        }
    };

    this.angleTo = function (v1, v2) {
        var dx = v1.x - v2.x,
            dy = v1.y - v2.y;
        return Math.atan2(dy, dx);
    };

    this.distanceTo = function (v1, v2) {
        var dx = v1.x - v2.x,
            dy = v1.y - v2.y;
        return Math.sqrt(dx * dx + dy * dy);
    };
}



