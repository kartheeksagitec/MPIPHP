/// <reference path="DataServ.js" />

var dashedLineFromTo = {
    isDrawing: true,
    unFinishedPixelsFromLastDash: 0
}
CanvasRenderingContext2D.prototype.dashedLineFromTo = function (from, to) {
    var x = from[0], y = from[1],
        dashLength = 2,
        dx = (to[0] - x) + .00000001,
        dy = to[1] - y,
        slope = dy / dx,
        distanceRemaining = Math.sqrt(dx * dx + dy * dy),
        bUnfinishedPixels = false,
        theDashLength,
        xStep;
    this.moveTo(x, y);
    while (distanceRemaining >= 0.1) {
        if (dashedLineFromTo.unFinishedPixelsFromLastDash === 0) {
            theDashLength = dashLength;
        } else {
            theDashLength = dashedLineFromTo.unFinishedPixelsFromLastDash;
            dashedLineFromTo.unFinishedPixelsFromLastDash = 0;
            dashedLineFromTo.isDrawing = !dashedLineFromTo.isDrawing
        }
        if (dashLength > distanceRemaining) { dashLength = distanceRemaining; bUnfinishedPixels = true; }
        xStep = Math.sqrt(theDashLength * theDashLength / (1 + slope * slope));
        x += xStep;
        y += slope * xStep;
        this[dashedLineFromTo.isDrawing ? 'lineTo' : 'moveTo'](x, y);
        distanceRemaining -= theDashLength;
        dashedLineFromTo.isDrawing = !dashedLineFromTo.isDrawing;
    }
    if (bUnfinishedPixels) {
        dashedLineFromTo.unFinishedPixelsFromLastDash = theDashLength;
    }
}
function Load() {

    raiseEvent(nsEvents.GetData);
}
function Render() {

    raiseEvent(nsEvents.RenderBPM);
}
function RenderReadOnlyBPM() {
    raiseEvent(nsEvents.RenderReadOnlyBPM);
}

function LoadMap(data) {
    ns.mapData["myCanvas"] = data;
    var mapCanvas = $('#myCanvas');
    var mapContext = mapCanvas[0].getContext('2d');
    mapCanvas.attr("width", data.Width);
    mapCanvas.attr("height", data.Height);
    LoadShapes(mapContext, data.lstShapes);
   // runMap(mapCanvas, mapContext, data);
    var btnDemo = $('#btnDemo');
    btnDemo[0].onclick = function () {
        $(this).prop('disabled', true);
        mapContext.clearRect(0, 0, mapCanvas.width, mapCanvas.height);
        ResetShapeExecutionStatus(data);
        mytimer = setInterval(function () {
            animate(mapCanvas, mapContext, data, false);
        }, 1000);
        ns.timers["myCanvas"] = mytimer;
        return false;
    }

    var btnRun = $('#btnRun');
    btnRun[0].onclick = function () {
        runMap(mapCanvas, mapContext, data);
        setTimeout(function () {
            alert("Execution completed.");
        }, 10);
        
        return false;
    }
}
function RenderReadOnlyBPMMap(data) {
    ns.mapData["myCanvas"] = data;
    var mapCanvas = $('#myCanvas');
    var mapContext = mapCanvas[0].getContext('2d');
    mapCanvas.attr("width", data.Width);
    mapCanvas.attr("height", data.Height);
    LoadShapes(mapContext, data.lstShapes);
    runMap(mapCanvas, mapContext, data);
    var btnDemo = $('#btnDemo');
    btnDemo[0].onclick = function () {
        mapContext.clearRect(0, 0, mapCanvas.width, mapCanvas.height);
        ResetShapeExecutionStatus(data);
        mytimer = setInterval(function () {
            animate(mapCanvas, mapContext, data, false);
        }, 1000);
        ns.timers["myCanvas"] = mytimer;
        return false;
    }
}
function RenderMap(data) {
    ns.mapData["myCanvas"] = data;
    var mapCanvas = $('#myCanvas');
    var mapContext = mapCanvas[0].getContext('2d');
    mapCanvas.attr("width", data.Width);
    mapCanvas.attr("height", data.Height);
    LoadShapes(mapContext, data.lstShapes);
    var btnDemo = $('#btnDemo');
   

    var btnRun = $('#btnRun');
    
}

function LoadShapes(acontext, alstShapes) {
    for (var ind in alstShapes) {
        var objShape = alstShapes[ind];
        if (objShape.ShapeName == "BPMNShape") {
            var isVerticalText = false;
            var isTask = false;
            var isTextAnnotation = false;
            switch (objShape.ShapeType) {
                case "participant":
                case "lane":
                    drawSquareParam(acontext, objShape);
                    isVerticalText = true;
                    break;
                case "task":
                case "userTask":
                    drawTask(acontext, objShape);
                    isTask = true;
                    break;
                case "serviceTask":
                    drawTask(acontext, objShape);
                    isTask = true;
                    break;
                case "businessRuleTask":
                    drawTask(acontext, objShape);
                    isTask = true;
                    break;
                case "manualTask":
                    drawTask(acontext, objShape);
                    isTask = true;
                    break;
                case "endEvent":
                case "startEvent":
                    drawCircle(acontext, objShape);
                    break;
                case "intermediateCatchEvent":
                    drawIntermediateCircle(acontext, objShape);
                    break;

                case "exclusiveGateway":
                    drawDiamond(acontext, objShape);
                    break;
                case "inclusiveGateway":
                    drawGateway(acontext, objShape);
                    break;
                case "parallelGateway":
                    drawGateway(acontext, objShape);
                    break;
                case "eventBasedGateway":
                    drawGateway(acontext, objShape);
                    break;

                case "callActivity":
                    drawCallActivity(acontext, objShape);
                    isTask = true;
                    break;
                case "textAnnotation":
                    drawTextAnnotation(acontext, objShape);
                    isTextAnnotation = true;
                    break;
                default:
                    drawTask(acontext, objShape);
                    break;
            }

            if (objShape.Text != "" && objShape.Text != null) {
                if (isVerticalText) {
                    drawVerticalText(acontext, objShape);
                }
                else {
                    var increment = 0;
                    if (isTask) {
                        increment = 29;
                        wrapText(acontext, objShape.Text, objShape.Left + 2, objShape.Top + increment, objShape.Width, 12, objShape.IsExecuted, false);
                    }
                    else if (isTextAnnotation) {
                        wrapText(acontext, objShape.Text, objShape.Left + 2, objShape.Top + 10, objShape.Width, 12, objShape.IsExecuted, true);
                    }
                    else {
                        wrapText(acontext, objShape.Text, objShape.LabelLeft + 5, objShape.LabelTop + 5, objShape.LabelWidth, 12, objShape.IsExecuted, false);
                    }
                }
            }
        }
        else if (objShape.ShapeName == "BPMNEdge") {
          
            if (objShape.ShapeType == "association") {
                AddAssociation(acontext, objShape);
            }
            else if (objShape.ShapeType == "messageFlow") {
                AddMessageFlow(acontext, objShape);
            }
            else {
            AddLineArray(acontext, objShape);
            }
            if (objShape.Text != "" && objShape.Text != null) {
                wrapText(acontext, objShape.Text, objShape.LabelLeft, objShape.LabelTop, objShape.LabelWidth, 12, objShape.IsExecuted);
            }
        }
    }
}

function AddAssociation(context, objShape) {
    var arrX = [];
    var arrY = [];
    for (var wayPointIndex in objShape.lstWayPoints) {
        var wayPoint = objShape.lstWayPoints[wayPointIndex];
        arrX.push(wayPoint.Left);
        arrY.push(wayPoint.Top);
    }
    context.save();
    context.lineWidth = 1;
    context.beginPath();
    for (i = 0; i < arrX.length - 1; i++) {
        if (arrX[i] > arrX[i + 1]) {
            context.dashedLineFromTo([arrX[i + 1], arrY[i + 1]], [arrX[i], arrY[i]]);
        }
        else {
            context.dashedLineFromTo([arrX[i], arrY[i]], [arrX[i + 1], arrY[i + 1]]);
        }
    }
    context.closePath();
    context.stroke();
    context.restore();
}
function AddMessageFlow(context, objShape) {
    var arrX = [];
    var arrY = [];
    for (var wayPointIndex in objShape.lstWayPoints) {
        var wayPoint = objShape.lstWayPoints[wayPointIndex];
        arrX.push(wayPoint.Left);
        arrY.push(wayPoint.Top);
    }
    context.save();
    context.lineWidth = 1;
    context.beginPath();
    for (i = 0; i < arrX.length - 1; i++) {
        if (arrX[i] > arrX[i + 1]) {
            context.dashedLineFromTo([arrX[i + 1], arrY[i + 1]], [arrX[i], arrY[i]]);
        }
        else {
            context.dashedLineFromTo([arrX[i], arrY[i]], [arrX[i + 1], arrY[i + 1]]);
        }
    }
    context.closePath();
    context.stroke();
    context.restore();
    var radians = Math.atan((arrY[arrY.length - 1] - arrY[arrY.length - 2]) / (arrX[arrX.length - 1] - arrX[arrX.length - 2]));
    radians += ((arrX[arrX.length - 1] >= arrX[arrX.length - 2]) ? 90 : -90) * Math.PI / 180;
    context.save();
    context.beginPath();
    context.translate(arrX[arrX.length - 1], arrY[arrY.length - 1]);
    context.rotate(radians);
    context.fillStyle = 'white';
    context.strokeStyle = 'black';
    context.moveTo(0, 0);
    context.lineTo(5, 5);
    context.lineTo(-5, 5);
    context.closePath();
    context.fill();
    context.stroke();
    context.restore();
    var startradians = Math.atan((arrY[1] - arrY[0]) / (arrX[1] - arrX[0]));
    startradians += ((arrX[1] >= arrX[0]) ? 90 : -90) * Math.PI / 180;
    context.save();
    context.beginPath();
    context.arc(arrX[0], arrY[0], 4, 0, 2 * Math.PI, false);
    context.fillStyle = 'white';
    context.fill();
    context.rotate(startradians);
    context.stroke();
    context.restore();
}
function drawTextAnnotation(context, objShape) {
    var radius = 5;
    Left = objShape.Left;
    Top = objShape.Top;
    Bottom = objShape.Height;
    Right = objShape.Width;
    context.moveTo(Left + 15, Top);
    context.lineTo(Left, Top);
    context.lineTo(Left, Top + Bottom);
    context.lineTo(Left + 15, Top + Bottom);
    context.stroke();
    context.restore();
}
function drawGateway(context, objShape) {
    var radius = 5;
    Left = objShape.Left;
    Top = objShape.Top;
    Bottom = objShape.Height;
    Right = objShape.Width;
    context.save();
    context.lineWidth = 1;
    context.beginPath();
    var point1 = {
        X: objShape.Left,
        Y: objShape.Top + (objShape.Height / 2)
    };
    var point2 = {
        X: objShape.Left + objShape.Width / 2,
        Y: objShape.Top
    };
    var point3 = {
        X: objShape.Left + objShape.Width,
        Y: objShape.Top + objShape.Height / 2
    };
    var point4 = {
        X: objShape.Left + objShape.Width / 2,
        Y: objShape.Top + objShape.Height
    };
    var arrX = [point1.X, point2.X, point3.X, point4.X, point1.X];
    var arrY = [point1.Y, point2.Y, point3.Y, point4.Y, point1.Y];
    context.moveTo(arrX[0], arrY[0]);
    for (i = 1; i < arrX.length; i++) {
        context.lineTo(arrX[i], arrY[i]);
    }
    context.lineJoin = 'round';
    if (objShape.IsCurrentShape) {
        context.fillStyle = "green";
    }
    else if (objShape.IsExecuted) {
        var my_gradient = context.createLinearGradient(0, objShape.Top, 0, objShape.Top + objShape.Height);
        my_gradient.addColorStop(0, "#FFE2AC");
        my_gradient.addColorStop(1, "#FFB32E");
        context.fillStyle = my_gradient;
    }
    else {
        context.fillStyle = "lightGray";
    }
    context.shadowColor = '#AAA';
    context.shadowBlur = 10;
    context.shadowOffsetX = 5;
    context.shadowOffsetY = 5;
    context.fill();
    context.stroke();
    SetGatewayImage(context, Left, Top, objShape.ShapeType);
    context.restore();
}
function SetGatewayImage(context, Left, Top, taskType) {
    var source = new Image();
    switch (taskType) {
        case "parallelGateway":
            source.src = '../BPMExecution/images/Parallel.svg';
            break;
        case "inclusiveGateway":
            source.src = '../BPMExecution/images/Inclusive.svg';
            break;
        case "eventBasedGateway":
            source.src = '../BPMExecution/images/Event.svg';
            break;
        case "intermediateCatchEvent":
            source.src = '../BPMExecution/images/Inclusive.svg';
            break;
    }
    source.onload = function () {
        switch (taskType) {
            case "inclusiveGateway":
            case "eventBasedGateway":
            case "parallelGateway":
                try {
                    context.drawImage(source, Left + 8, Top + 8);
                }
                catch (err) {
                    setTimeout(function () { context.drawImage(source, Left + 8, Top + 8) }, 1);
                }
                break;
            case "intermediateCatchEvent":
                try {
                    context.drawImage(source, Left - 13, Top - 13);
                }
                catch (err) {
                    setTimeout(function () { context.drawImage(source, Left - 13, Top - 13) }, 1);
                }
        }
    }
}
function drawTask(context, objShape) {
    var radius = 5;
    Left = objShape.Left;
    Top = objShape.Top;
    Bottom = objShape.Height;
    Right = objShape.Width;

    context.save();
    context.beginPath();
    context.moveTo(Left + radius, Top);
    context.lineTo(Left + Right - radius, Top);
    context.quadraticCurveTo(Left + Right, Top, Left + Right, Top + radius);
    context.lineTo(Left + Right, Top + Bottom - radius);
    context.quadraticCurveTo(Left + Right, Top + Bottom, Left + Right - radius, Top + Bottom);
    context.lineTo(Left + radius, Top + Bottom);
    context.quadraticCurveTo(Left, Top + Bottom, Left, Top + Bottom - radius);
    context.lineTo(Left, Top + radius);
    context.quadraticCurveTo(Left, Top, Left + radius, Top);
    context.closePath();
    context.lineWidth = 1;
    context.strokeStyle = 'black';
   
    if (objShape.IsCurrentShape) {
        context.fillStyle = "green";
    }
    else if (objShape.IsExecuted) {
        var my_gradient = context.createLinearGradient(0, Top, 0, Top + Bottom);
        my_gradient.addColorStop(0, "#eaffad");
        my_gradient.addColorStop(1, "#cfff4a");
        context.fillStyle = my_gradient;

    }
    else {
        context.fillStyle = "lightGray";
    }

    context.shadowColor = '#AAA';
    context.shadowBlur = 10;
    context.shadowOffsetX = 5;
    context.shadowOffsetY = 5;

    context.fill();
    context.stroke();
    SetTaskImage(context, Left, Top, objShape.ShapeType);
    context.restore();
}

function SetTaskImage(context, Left, Top, taskType) {
    var source = new Image();
    switch (taskType) {
        case "userTask":
            source.src = '../BPMExecution/images/user-task.svg';
            source.onload = function () {
                try {
                    context.drawImage(source, Left + 1, Top + 1);
                }
                catch (err) {
                    setTimeout(function () { context.drawImage(source, Left + 1, Top + 1) }, 1);
                }
            }
            break;
        case "serviceTask":
            source.src = '../BPMExecution/images/service-task.svg';
            source.onload = function () {
                try {
                    context.drawImage(source, Left + 1, Top + 1);
                }
                catch (err) {
                    setTimeout(function () { context.drawImage(source, Left + 1, Top + 1) }, 1);
                }
            }
            break;
        case "businessRuleTask":
            source.src = '../BPMExecution/images/business-rule-task.svg';
            source.onload = function () { context.drawImage(source, Left + 1, Top + 1); }
            break;
        case "manualTask":
            source.src = '../BPMExecution/images/manual-task.svg';
            source.onload = function () { context.drawImage(source, Left + 1, Top + 1); }
            break;
    }
}

function drawSquareParam(context, objShape) {
    Left = objShape.Left;
    Top = objShape.Top;
    Bottom = objShape.Height;
    Right = objShape.Width;
    context.save();
    context.beginPath();
    context.rect(Left, Top, Right, Bottom);
    context.fillStyle = '#F3F3F3';
    context.fill();
    context.restore();
    context.lineWidth = 1;
    context.strokeStyle = 'black';
    context.stroke();
    if (objShape.ShapeType == 'participant') {
        var my_gradient = context.createLinearGradient(Left, 0, Left + 40, 0);
        my_gradient.addColorStop(0, "#C9C9C9");
        my_gradient.addColorStop(0.5, "#EAEAEA");
        my_gradient.addColorStop(1, "#C9C9C9");
        context.fillStyle = my_gradient;
    context.fill();
    context.restore();
    }
    //drawing line
    context.save();
    context.beginPath();
    context.moveTo(Left + 40, Top);
    context.lineTo(Left + 40, Bottom + Top);
    context.fillStyle = '#FFFFFF'
    context.lineWidth = 1;
    context.strokeStyle = 'black';
    context.stroke();
    context.fill();
    context.restore();

}

function AddLineArray(context, objShape) {

    var arrX = [];
    var arrY = [];
    for (var wayPointIndex in objShape.lstWayPoints) {
        var wayPoint = objShape.lstWayPoints[wayPointIndex];
        arrX.push(wayPoint.Left);
        arrY.push(wayPoint.Top);
    }

    context.save();
    context.lineWidth = 1;
    context.beginPath();
    context.moveTo(arrX[0], arrY[0]);
    for (i = 1; i < arrX.length; i++) {
        context.lineTo(arrX[i], arrY[i]);
    }
    context.lineJoin = 'round';
    if (objShape.IsCurrentShape) {
        context.strokeStyle = "green";
    }
    else if (!objShape.IsExecuted) {
        context.strokeStyle = "lightGray";
    }
   
    context.stroke();
    context.restore();
    var radians = Math.atan((arrY[arrY.length - 1] - arrY[arrY.length - 2]) / (arrX[arrX.length - 1] - arrX[arrX.length - 2]));
    radians += ((arrX[arrX.length - 1] >= arrX[arrX.length - 2]) ? 90 : -90) * Math.PI / 180;
    context.save();
    context.beginPath();
    context.translate(arrX[arrX.length - 1], arrY[arrY.length - 1]);
    context.rotate(radians);
    context.moveTo(0, 0);
    context.lineTo(5, 5);
    context.lineTo(-5, 5);
    context.closePath();
    if (objShape.IsCurrentShape) {
        context.strokeStyle = "green";
    }
    else if (!objShape.IsExecuted) {
        context.fillStyle = "lightGray";
    }
   
    context.fill();
    context.restore();
}

function drawDiamond(context, objShape) {
    context.save();
    context.lineWidth = 1;
    context.beginPath();

    var point1 = {
        X: objShape.Left,
        Y: objShape.Top + (objShape.Height / 2)
    };
    var point2 = {
        X: objShape.Left + objShape.Width / 2,
        Y: objShape.Top
    };
    var point3 = {
        X: objShape.Left + objShape.Width,
        Y: objShape.Top + objShape.Height / 2
    };
    var point4 = {
        X: objShape.Left + objShape.Width / 2,
        Y: objShape.Top + objShape.Height
    };
    var arrX = [point1.X, point2.X, point3.X, point4.X, point1.X];
    var arrY = [point1.Y, point2.Y, point3.Y, point4.Y, point1.Y];

    context.moveTo(arrX[0], arrY[0]);
    for (i = 1; i < arrX.length; i++) {
        context.lineTo(arrX[i], arrY[i]);
    }
    context.lineJoin = 'round';
    
    if (objShape.IsCurrentShape) {
        context.fillStyle = "green";
    }
    else if (objShape.IsExecuted) {
        var my_gradient = context.createLinearGradient(0, objShape.Top, 0, objShape.Top+objShape.Height);
        my_gradient.addColorStop(0, "#FFE2AC");
        my_gradient.addColorStop(1, "#FFB32E");
        context.fillStyle = my_gradient;
    }
    else {
        context.fillStyle = "lightGray";
    }
    context.shadowColor = '#AAA';
    context.shadowBlur = 10;
    context.shadowOffsetX = 5;
    context.shadowOffsetY = 5;
    context.fill();
    context.stroke();
    context.restore();
}

function drawCircle(context, objShape) {
    var centerX = objShape.Left + 15;
    var centerY=objShape.Top + 15;
    var radius = 16;
    context.save();
    context.beginPath();
    context.arc(centerX, centerY, radius, 0, 2 * Math.PI, false);
    context.lineWidth = 1;
    context.stroke();
    if (objShape.IsCurrentShape) {
        context.fillStyle = "green";
    }
    else if (objShape.IsExecuted) {
        var my_gradient = context.createRadialGradient(centerX, centerY, radius, centerX, centerY - 10, 0);
        if (objShape.ShapeType == "startEvent") {
            my_gradient.addColorStop(0, "#5E720C");
            my_gradient.addColorStop(1, "#E0FF5E");
        }
        else if (objShape.ShapeType == "endEvent") {
            my_gradient.addColorStop(0, "#800E0E");
            my_gradient.addColorStop(1, "#FF5555");
        }
        else {
            my_gradient.addColorStop(0, "#FFBA0F");
            my_gradient.addColorStop(1, "#FFD780");
        }
        context.fillStyle = my_gradient;
    }
    else {
        context.fillStyle = "lightGray";
    }
    context.shadowColor = '#AAA';
    context.shadowBlur = 10;
    context.shadowOffsetX = 5;
    context.shadowOffsetY = 5;
    context.fill();
    context.restore();
}

function drawIntermediateCircle(context, objShape) {
    Left = objShape.Left;
    Top = objShape.Top;
    Bottom = objShape.Height;
    Right = objShape.Width;
    var centerX = objShape.Left + 15;
    var centerY = objShape.Top + 15;
    var radius = 16;
    context.save();
    context.beginPath();
    context.arc(centerX, centerY, radius, 0, 2 * Math.PI, false);
    context.lineWidth = 1;
    context.stroke();
    if (objShape.IsCurrentShape) {
        context.fillStyle = "green";
    }
    else if (objShape.IsExecuted) {
        var my_gradient = context.createRadialGradient(centerX, centerY, radius, centerX, centerY - 10, 0);
        if (objShape.ShapeType == "startEvent") {
            my_gradient.addColorStop(0, "#5E720C");
            my_gradient.addColorStop(1, "#E0FF5E");
        }
        else if (objShape.ShapeType == "endEvent") {
            my_gradient.addColorStop(0, "#800E0E");
            my_gradient.addColorStop(1, "#FF5555");
        }
        else {
            my_gradient.addColorStop(0, "#FFBA0F");
            my_gradient.addColorStop(1, "#FFD780");
        }
        context.fillStyle = my_gradient;
    }
    else {
        context.fillStyle = "lightGray";
    }
    context.shadowColor = '#AAA';
    context.shadowBlur = 10;
    context.shadowOffsetX = 5;
    context.shadowOffsetY = 5;
    context.fill();
    context.stroke();
    SetGatewayImage(context, centerX, centerY, objShape.ShapeType)
    context.restore();
}
function wrapText(context, text, x, y, maxWidth, lineHeight, isExecuted) {
    var words = text.split(' ');
    var line = '';
    context.save();
    for (var n = 0; n < words.length; n++) {
        var testLine = line + words[n] + ' ';
        var metrics = context.measureText(testLine);
        var testWidth = metrics.width;
        if (testWidth > maxWidth && n > 0) {
            context.fillStyle = "black";
            context.fillText(line, x, y);
            line = words[n] + ' ';
            y += lineHeight;
        }
        else {
            line = testLine;
        }
    }
    context.fillStyle = "black";
    context.fillText(line, x, y);
    context.restore();
}

function drawVerticalText(context, objShape) {
    context.save();
    var x = objShape.Left;
    var y = objShape.Top;
    var height = objShape.Height;
    var text = objShape.Text;
    var lineHeight = 12;
    var words = text.split(' ');
    var line = '';
    var yval = 0;
    var lstData = [];
    var lstYvalues = [];
    var index = 0;
    var metrics;
    for (var n = 0; n < words.length; n++) {
        var testLine = line + words[n] + ' ';
        metrics = context.measureText(testLine);
        var testWidth = metrics.width;
        if (testWidth > height && n > 0) {
            lstData[index] = line;
            lstYvalues[index] = yval;
            line = words[n] + ' ';
            yval += lineHeight;
            index++;
        }
        else {
            line = testLine;
        }
    }
    lstData[index] = line;
    lstYvalues[index] = yval;
    var rad = 270 * (Math.PI / 180);
    var tx = x + 20;
    var ty;
    if (lstData.length == 1) {
        metrics = context.measureText(text);
        var txtwidth = metrics.width;
        ty = y + ((height) - (height - txtwidth) / 2);
    }
    else {
        ty = y + height - 10;
    }
    context.translate(tx, ty);
    context.rotate(rad);
    context.font = "9pt Segoe UI"
    context.fillStyle = "black";
    context.textAlign = "start";
    //context.fillText(text, 0, 0);
    for (var ind = 0; ind < lstData.length; ind++) {
        context.fillText(lstData[ind], 0, lstYvalues[ind]);
    }
    context.rotate(rad * (-1));
    context.translate(-tx, -ty);
    context.restore();
}

function drawCallActivity(context, objShape) {
    var radius = 5;
    Left = objShape.Left;
    Top = objShape.Top;
    Bottom = objShape.Height;
    Right = objShape.Width;

    context.save();
    context.beginPath();
    context.moveTo(Left + radius, Top);
    context.lineTo(Left + Right - radius, Top);
    context.quadraticCurveTo(Left + Right, Top, Left + Right, Top + radius);
    context.lineTo(Left + Right, Top + Bottom - radius);
    context.quadraticCurveTo(Left + Right, Top + Bottom, Left + Right - radius, Top + Bottom);
    context.lineTo(Left + radius, Top + Bottom);
    context.quadraticCurveTo(Left, Top + Bottom, Left, Top + Bottom - radius);
    context.lineTo(Left, Top + radius);
    context.quadraticCurveTo(Left, Top, Left + radius, Top);
    context.closePath();
    context.lineWidth = 3;
    context.strokeStyle = 'black';
    if (objShape.IsCurrentShape) {
        context.fillStyle = "green";
    }
    else if (objShape.IsExecuted) {
        var my_gradient = context.createLinearGradient(0, Top, 0,Top+Bottom) ;
        my_gradient.addColorStop(0, "#eaffad");
        my_gradient.addColorStop(1, "#cfff4a");
        context.fillStyle = my_gradient;
    }
    else {
        context.fillStyle = "lightGray";
    }

    context.shadowColor = '#AAA';
    context.shadowBlur = 10;
    context.shadowOffsetX = 5;
    context.shadowOffsetY = 5;

    context.fill();

    context.stroke();
    var source = new Image();
    source.src = '../BPMExecution/images/calling-process.svg';

    var imgLeft = Left + (Right / 2);
    var imgTop = (Top + Bottom) - 10
    source.onload = function () {
        try {
        context.drawImage(source, imgLeft, imgTop);
        }
        catch (err) {
            setTimeout(function () { context.drawImage(source, imgLeft, imgTop) }, 1);
        }
    }
    context.restore();
}

function OnDoubleClick(e) {

    var cursor = getMousePos(e.srcElement, e);
    var mapData = ns.mapData[e.srcElement.id];
    if (mapData != undefined) {
        for (var name in mapData.lstShapes) {
            objShape = mapData.lstShapes[name];
            var Right = objShape.Left + objShape.Width;
            var Bottom = objShape.Top + objShape.Height;
            if (objShape.ShapeType == "callActivity") {
                if (cursor.x > objShape.Left && cursor.x < Right && cursor.y > objShape.Top && cursor.y < Bottom) {
                    var isFound = false;
                    for (var itm in mapData.lstExecutedSteps) {
                        var objStep = mapData.lstExecutedSteps[itm];
                        if (objStep.ElementId == objShape.Id) {
                            isFound = true;
                            ShowCallActivityMap(objStep, objShape, false);
                        }
                    }
                    if (!isFound) {
                        alert("step is not executed..");
                    }
                    break;
                }
            }
        }
    }

}

function showVariables(canvas,left, top) {
    var rect = canvas.getBoundingClientRect();
    var cursor = {
        x: left - rect.left,
        y: top- rect.top
    };

    var mapData = ns.mapData[canvas.id];
    if (mapData != undefined) {
        for (var name in mapData.lstShapes) {
            objShape = mapData.lstShapes[name];
            if (objShape.IsExecuted) {
                var Right = objShape.Left + objShape.Width;
                var Bottom = objShape.Top + objShape.Height;
                if (cursor.x > objShape.Left && cursor.x < Right && cursor.y > objShape.Top && cursor.y < Bottom) {
                    for (var itm in mapData.lstExecutedSteps) {
                        var objStep = mapData.lstExecutedSteps[itm];
                        if (objStep.ElementId == objShape.Id) {
                            ShowVariablesPopup(objStep);
                        }
                    }
                }
            }
        }
    }
}

function getMousePos(canvas, evt) {
    var rect = canvas.getBoundingClientRect();
    return {
        x: evt.clientX - rect.left,
        y: evt.clientY - rect.top
    };
}

function OpenPopup(data, isRun) {

    Wid = data.WindowID;
    var CanvasID = Wid + "Canvas";
    var DivHtml = '<div id="' + Wid + '"> <canvas id="' + CanvasID + '" width="' + data.Width + '" height="' + data.Height + '" ondblclick="OnDoubleClick(event)"/><div>';
    $("#Container").append(DivHtml);

    var window2open = $("#" + Wid);
    window2open.kendoWindow({
        width: "600px",
        height: "600px",
        title: data.Title,
        actions: [
                     "Minimize",
                     "Maximize",
                     "Close"
        ]
    });

    //if (objParentShape != undefined) {
    //    objParentShape.CallActivityDetails = data;
    //    //for (var ind in data.lstShapes) {
    //    //    objParentShape.lstShapes.push(data.lstShapes[ind]);
    //    //}
    //}
    var callActivityMapCanvas = $("#" + CanvasID)
    context = callActivityMapCanvas[0].getContext('2d');
    ns.mapData[CanvasID] = data;
    LoadShapes(context, data.lstShapes);
    window2open.data("kendoWindow").open().maximize();

    if (isRun != undefined && isRun) {
        var callActivitytimer = setInterval(function () { animate(callActivityMapCanvas, context, data, true, window2open.data("kendoWindow")); }, 1000);
        ns.timers[callActivityMapCanvas.attr("id")] = callActivitytimer;
    }




}

function UpdateCurrentShapeStatus(data) {
    if (data.ExecutedStepIndex + 1 < data.lstExecutedSteps.length) {
        var objExecutedStep = data.lstExecutedSteps[data.ExecutedStepIndex + 1];
        var objShape = getExecutedShape(objExecutedStep, data.lstShapes);
        if (null != objShape) {
            objShape.IsCurrentShape = true;
        }
    }
}

function animate(canvas, context, data, isCallActivity, kendoWindow) {
    var curTimer;
    if (data.ExecutedStepIndex < data.lstExecutedSteps.length) {

        var objExecutedStep = data.lstExecutedSteps[data.ExecutedStepIndex];
        var objShape = getExecutedShape(objExecutedStep, data.lstShapes);
        if (null != objShape) {
            objShape.IsCurrentShape = false;
            UpdateCurrentShapeStatus(data);
            if (objShape.ShapeType == "callActivity") {

                curTimer = ns.timers[canvas.attr("id")];
                clearInterval(curTimer);
                var parentDetails = {
                    Canvas: canvas,
                    Context: context,
                    Data: data,
                    IsCallActivity: isCallActivity,
                    KendoWindow: kendoWindow
                };
                ns.Parent.push(parentDetails);
                ShowCallActivityMap(objExecutedStep, objShape, true);

            }

            objShape.IsExecuted = true;
            context.clearRect(0, 0, canvas.width, canvas.height);
            LoadShapes(context, data.lstShapes);
        }

        data.ExecutedStepIndex++;
    }
    else {
        data.ExecutedStepIndex = 0;
        curTimer = ns.timers[canvas.attr("id")];
        clearInterval(curTimer);
        if (isCallActivity) {
            if (kendoWindow != undefined) {
                kendoWindow.close();
                delete ns.timers[canvas.attr("id")];//deleting timer instance after completion
                var parentObj = ns.Parent.pop();
                if (parentObj != undefined) {

                    objTimer = setInterval(function () { animate(parentObj.Canvas, parentObj.Context, parentObj.Data, parentObj.IsCallActivity, parentObj.KendoWindow); }, 1000);
                    ns.timers[parentObj.Canvas.attr("id")] = objTimer;
                }
            }
        }
        else {
            alert("Execution completed.");
            $('#btnDemo').prop('disabled', false);
        }
    }

}

function ShowCallActivityMap(objStep, objShape, isRun) {

    var reqObject = {
        Type: "POST",
        Action: "GetCallActivityData",
        IsRun: isRun,
        Param:
            {
                astrXMLFile: objStep.XMLFile,
                lstExecutedSteps: objStep.lstExecutedSteps,
                Title: objShape.Text,
                IsExecuted: objShape.IsExecuted
            },

    };
    ns.CurrentRequestObj = reqObject;
    raiseEvent(nsEvents.GetCallActivityData);
}

function getExecutedShape(objExecutedStep, lstShapes) {
    var shape = null;
    for (var ind in lstShapes) {
        var objShape = lstShapes[ind];
        if (objExecutedStep.ElementId == objShape.Id) {
            shape = objShape;
            break;
        }
    }

    return shape;
}

function runMap(canvas, context, data) {
    context.clearRect(0, 0, canvas.width, canvas.height);
    ResetShapeExecutionStatus(data);
    for (var ind in data.lstShapes) {
        objShape = data.lstShapes[ind];
        var isFound = false;
        for (var itm in data.lstExecutedSteps) {
            var objStep = data.lstExecutedSteps[itm];
            if (objStep.ElementId == objShape.Id) {
                isFound = true;
            }
        }

        if (isFound) {
            objShape.IsExecuted = true;
        }
    }
    LoadShapes(context, data.lstShapes);
    //alert("Execution completed.");
}

function ResetShapeExecutionStatus(data) {
    for (var ind in data.lstShapes) {
        data.lstShapes[ind].IsExecuted = false;
        data.lstShapes[ind].IsCurrentShape = false;
    }
}

function ShowVariablesPopup(objStep) {

    var window2open = $("#variables");
    var table = $("#tableVariables");

    for (var ind in objStep.Parameters) {
        var objParam = objStep.Parameters[ind];

        var row = table[0].insertRow(table[0].rows.length);
        // Insert new cells (<td> elements) at the 1st and 2nd position of the "new" <tr> element:
        var cell1 = row.insertCell(0);
        var cell2 = row.insertCell(1);
        var cell3 = row.insertCell(2);

        // Add some text to the new cells:
        cell1.innerHTML = objParam['<istrParameterName>k__BackingField'];
        cell2.innerHTML = objParam['<istrParameterDataType>k__BackingField'];
        cell3.innerHTML = objParam['<istrParameterValue>k__BackingField'];
    }


    window2open.kendoWindow({
        width: "600px",
        height: "400px",
        title: "Variables",
        position: { top: 100,left:100 },
        actions: [
                     "Minimize",
                     "Maximize",
                     "Close"
        ]
    });

    window2open.data("kendoWindow").open();
}
