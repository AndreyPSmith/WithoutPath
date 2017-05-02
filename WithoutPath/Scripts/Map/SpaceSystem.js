/**
 * Wormhole
 */
var SpaceSystem = function (data) {

    this.Id = 0;
    this.Map = null;
    this.Name = "Unknown";
    this.EveID = null;
    this.isHome = false;
    this.isWarning = false;
    this.Note = null;
    this.radius = 0;
    this.x = 0;
    this.y = 0;
    this.Type = "";
    this.Class = "";
    this.Security = null;
    this.Pilots = [];
    this.Effect = "No effects";
    this.IsInHeap = false;
    this.LinkedToHome = true;
    this.Statics = [];

    if (data) {
        this.Map = data.Map;
        this.Id = data.Id;
        this.radius = data.radius != null ? data.radius : this.radius;
        this.isHome = data.isHome != null ? data.isHome : this.isHome;
        this.isWormhole = data.isWormhole? true : false;
        this.isWarning = data.isWarning != null ? data.isWarning : this.isWarning;
        this.Note = data.Note != null ? data.Note : this.Note;
        this.Name = data.Name != null ? data.Name : this.Name;
        this.EveID = data.EveID != null ? data.EveID : this.EveID;
        this.Type = data.Type != null ? data.Type : this.Type;
        this.Class = data.Class != null ? data.Class : this.Class;
        this.Security = data.Security != null ? data.Security : this.Security;
        this.Pilots = data.Pilots != null ? data.Pilots : this.Pilots;
        this.Effect = data.Effect != null ? data.Effect : this.Effect;
        this.Statics = data.Statics != null ? data.Statics : this.Statics;
        this.Statics = data.Statics != null ? data.Statics : this.Statics;
    }

    this.currentRadius = this.radius * 0.9;

    this.isMouseOver = false;
    this.dragging = false;
    this.destroyed = false;
    this._easeRadius = 0;
    this._dragDistance = null;
    this.collapsing = false;
    this.IsMovedToPoint = false;
    this._currentEase = Math.floor(Math.random() * (100 - 10)) + 10;
    this._easeIsGrow = false;
    this.arcRenderFinish = false;
    this.canStartRenderArc = false;
    this.incomingAngle = 90;

    this.IsTagVisible = true;
    this._IsTagRendered = 0;
    this._TagToShelfProgress = 0;
    this._TagShelfProgress = 0;
    this._TagOpacity = 0;

    this.Impl = this.isWormhole ? new Wormhole(this) : new Empire(this);
}

SpaceSystem.prototype = {

    GetName: function () {
        return this.Impl.GetName();
    },

    GetPoint: function () {
        return this.Impl.GetPoint();
    },

    Point: function () {
        return Point = { x: this.x, y: this.y };
    },

    render: function () {
        this.Impl.render();
    },

    renderTag: function () {
        this.Impl.renderTag();
    },

    renderArc: function () {
        this.Impl.renderArc();
    },

    moveToPoint: function (Point, animate) {
        this.Impl.moveToPoint(Point, animate);
        this.IsMovedToPoint = true;
    },

    collapse: function () {
        this.Impl.collapse();
    },

    SetDraggable: function (draggable) {
        this.Impl.SetDraggable(draggable);
    }
};

