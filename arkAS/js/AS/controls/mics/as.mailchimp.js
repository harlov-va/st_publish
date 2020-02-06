var as = as || {};

as.mailchimp = {
    options: {
        appendTo: 'body'
    },

    init: function (options) {
        as.mailchimp.options = $.extend(as.mailchimp.options, options);
    },
    
    loadByInfo: function () {
        $.ajax({
            type: 'GET',
            dataType: 'json',
            url: "/Mailchimp/GetAllLists",
            data: {},
            success: function (data) {
                if (data.result) {
                    as.mailchimp.viewResult(data.result);
                }
            }
        });
    },
    viewResult: function (result) {
        var obj = JSON.parse(result);
        var s = [];
        s.push("<br />");
        s.push("<table class='table-bordered'><thead><tr><th>ID</th><th>Name</th><th>Company</th><th>Email</th></tr></thead><tbody>");
        $.each(obj.lists, function (index) {
            s.push("<tr>");
            s.push("<td class='item'>" + obj.lists[index].id + "</td><td>" + obj.lists[index].name +"</td><td>" + obj.lists[index].contact.company + "</td><td>" + obj.lists[index].contact.address1 + "</td>");
            s.push("</tr>");
        });
        s.push("</tbody></table>");
        $(as.mailchimp.options.appendTo).empty().append(s.join(""));
    }
};

 