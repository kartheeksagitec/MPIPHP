var constShowDelay = 300; //ms- configurable
var constDisappearDelay = 300; //ms- configurable
var myVar;
var myTimeoutID;
var myNode, myData;
var ref_Menu_HoverStatic;
var ref_Menu_Unhover;
var ref_overrideMenu_HoverStatic;
// This function is called in <body onload="...">
function initMenuMouseHoverInterceptors() {
    // *** Interceptors ***
    // @:: Menu_Hover
    //debugger;
    //handle case if no menu on the page
    if ((typeof (Menu_HoverStatic) != 'undefined')) {
        ref_Menu_HoverStatic = Menu_HoverStatic;
        Menu_HoverStatic = My_Menu_HoverStatic;
        // @:: Menu_Unhover
        ref_Menu_Unhover = Menu_Unhover;
        Menu_Unhover = My_Menu_Unhover;
        // @:: overrideMenu_HoverStatic
        ref_overrideMenu_HoverStatic = Menu_HoverStatic; //corrected by skynyrd
        Menu_HoverStatic = My_overrideMenu_HoverStatic;
    }
}
function My_Menu_HoverStatic(item) {    
    My_overrideMenu_HoverStatic(item);
}
function My_overrideMenu_HoverStatic(item) {
    var node = Menu_HoverRoot(item);
    var data = Menu_GetData(item);
    myNode = node;
    myData = data;
    if (!data) return;
    myVar = item;
    myTimeoutID = setTimeout("My_DelayExpandMenu(myNode,myData)", constShowDelay); //COnfigurable
}
function My_DelayExpandMenu(node, data) {
    __disappearAfter = constDisappearDelay; //data.disappearAfter;
    Menu_Expand(node, data.horizontalOffset, data.verticalOffset);
}
function My_Menu_Unhover(item) {
    clearTimeout(myTimeoutID);
    ref_Menu_Unhover(item);
}
//   <!-- ~END:: JavaScript to prevent the expanding of static menu when you quickly mouse over them -->