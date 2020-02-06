var as = as || {};

as.vacancy = {
    options: {
        appendTo: 'body',
    },
    params_id: "",

    init: function (options) {
        as.vacancy.options = $.extend(as.vacancy.options, options);
        var opts = as.vacancy.options;

    },
    load: function ( keywords, region, levelSolary, experience, currency) {
        $.ajax({
            url: "/Vacancy/GetVacancyParams",
            type: 'GET',
            dataType: 'json',
            data: { keywords: keywords, region: region, levelSolary: levelSolary, experience:experience , currency:currency },       
            success: function (data) {
               console.log(data);
               as.vacancy.viewResult(data);
                }           
        });
    },
    loadKeywords: function (keywords) {
        $.ajax({
            url: "/Vacancy/GetVacancyParamsKeywords",
            type: 'GET',
            dataType: 'json',
            data: { keywords: keywords },
            success: function (data) {
                console.log(data);
                as.vacancy.viewResult(data);
            }
        });
    },
    areas: function () {
        $.ajax({
            url: "/Vacancy/GetSity",
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                as.vacancy.GetId(data);
            }
        });
    },
     GetId: function(data) {
            var json = JSON.parse(data);
            var id = "";
            var s = [];
            s.push('<option value=' + json[0].areas[1].areas[3].id + '>' + json[0].areas[1].areas[3].name + '</option>');
            s.push('<option value=' + json[0].areas[40].id + '>' + json[0].areas[40].name + '</option>');
            s.push('<option value=' + json[0].areas[6].id + '>' + json[0].areas[6].name + '</option>');
            s.push('<option value=' + json[0].areas[16].areas[15].id + '>' + json[0].areas[16].areas[15].name + '</option>');
            s.push('<option value=' + json[0].areas[60].areas[3].id + '>' + json[0].areas[60].areas[3].name + '</option>');
            s.push('<option value=' + json[0].areas[73].areas[10].id + '>' + json[0].areas[73].areas[10].name + '</option>');
            $(as.vacancy.options.appendTo).empty().append(s.join(""));
        },
    

    viewResult: function (result) {
        var obj = JSON.parse(result);

        var s = [];
        for (var i = 0; i < obj.items.length; i++)
        {
            s.push('<div class="name_item"><p = class="lead name_legend"><strong>' + obj.items[i].name + '</strong></p></div>');
            if (obj.items[i].salary != null && obj.items[i].salary.from != null)
                s.push("<div class='salary_item'><p class='salary_legend'>" + "От " + obj.items[i].salary.from + "</p></div>");
            if (obj.items[i].snippet.requirement != null)
                s.push("<div class='requirement_item'><p>" + obj.items[i].snippet.requirement);
            if (obj.items[i].snippet.requirement != null)
            s.push(obj.items[i].snippet.responsibility + "</p></div>");
            s.push("<div class='employer_item'><p>" +"Фирма:"+ obj.items[i].employer.name + "</p></div>");
        }
        $(as.vacancy.options.appendTo).empty().append(s.join(""));
    },


}