// this is code to make divs with the "editable" class be updateable. The id must be the key of the target in the database, and 
// a StringsPut.cshtml file must exist to update the database

jQuery(".editable").click(function onEditableClick() {
    var oEditable = jQuery(this);
    var sId = this.id;
    var sOld = jQuery(this).html();

    // adding new buttons to wikified html
    jQuery(this).html('<textarea id="editText">' + Wiky.toWiki(sOld) +
        '</textarea><input id="updatebuttonid" type="button" value="Update" /><input id="cancelbuttonid" type="button" value="Cancel" />');
    jQuery("#editText").autoGrow();
    jQuery("#editText").focus();

    // turn editing of other fields off
    jQuery(".editable").unbind("click");
    jQuery("#updatebuttonid").click(function () {
        var oButton = jQuery(this);

        // get text out of text area
        var sText = jQuery("#editText").val();
        oEditable.html(Wiky.toHtml(sText));

        // saving string
        jQuery("#outputArea").html("<img src=\"images/turningArrow.gif\" />");
        var oData = { sKey: sId,
            sValue: oEditable.html()
        };
        jQuery("#outputArea").load("StringsPut.cshtml", oData);

        // turn editing back on
        jQuery(".editable").click(onEditableClick);
        return false;
    });
    jQuery("#cancelbuttonid").click(function () {
        // restoring contents of div and turn editing back on
        oEditable.html(sOld);
        jQuery(".editable").click(onEditableClick);
        return false;
    });
});
