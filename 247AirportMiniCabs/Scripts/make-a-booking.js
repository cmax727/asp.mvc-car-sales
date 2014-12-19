var locationFor;

function showDropDown(target, dropDown){
	target = $('#' + target)[0];
	dropDown = $('#' + dropDown)[0];
	
	var top = $(target).offset().top + target.clientHeight,
	left = $(target).offset().left + 1;
	
	$(dropDown).css('top', top).css('left', left).css('width', target.clientWidth).slideDown();
}

var airports, stations, hotels, famousPlaces;

function getOptionsByCategory(category, callback){
	$.post('/Defaults/GetLocationsByCategory?category=' + category, function(response){
		var options = '';
		$(response).each(function(index, location){
		    options += '<option accesskey="' + location.postcode + '" id="'+ location.address +'">' + location.name + '</option>';
		});
		callback(options);
	});
}

getOptionsByCategory('Airport', function(options){
	airports = options;
	$('#select-pick-from').html(airports);
});

getOptionsByCategory('Station', function(options){
	stations = options;
});

getOptionsByCategory('Hotel', function(options){
	hotels = options;
});

getOptionsByCategory('Famous Place', function(options){
	famousPlaces = options;
});

function showTextElement(element, value) {
    $(element).find('input[type=text]').val(value).show().focus();
    $(element).find('.search-uk-addresses').show();
    $(element).find('.options-list, textarea, #postcodes').hide();
}

function showSelectElement(element, options){
    $(element).find('input[type=text], textarea, .search-uk-addresses, #postcodes').hide();
    $(element).find('.options-list').html(options).show().focus();
}

function postalCodesHandling(element){
	showTextElement(element, '');
}

function airportsHandling(element){
	showSelectElement(element, airports);
}

function hotelsHandling(element){
	showSelectElement(element, hotels);
}

function trainStationsHandling(element){
	showSelectElement(element, stations);
}

function famousPlacesHandling(element){
	showSelectElement(element, famousPlaces);
}

function getQuoteClick() {

    $(postCodeElement).hide();

    var inputPickFrom = $('#input-pick-from'), inputDropTo = $('#input-drop-to');

    var fromFullAddress = '', toFullAddress = '', selectedElement, selectedPostCode;

    if ($(inputPickFrom).css('display') !== 'none') {
        fromFullAddress = $(inputPickFrom).val();
        inputPickFrom = $(inputPickFrom).attr('accesskey');
    }
    else if ($('#select-pick-from').css('display') !== 'none') {
        selectedElement = $('#select-pick-from')[0];
        selectedElement = $(selectedElement).find('option')[selectedElement.selectedIndex];
        inputPickFrom = $(selectedElement).attr('accesskey');
        fromFullAddress = $(selectedElement).attr('id');
    }
    else
        fromFullAddress = inputPickFrom = $('#textarea-pick-from').val();

    if ($(inputDropTo).css('display') !== 'none') {
        toFullAddress = $(inputDropTo).val();
        inputDropTo = $(inputDropTo).attr('accesskey');
    }
    else if ($('#select-drop-to').css('display') !== 'none') {
        selectedElement = $('#select-drop-to')[0];
        selectedElement = $(selectedElement).find('option')[selectedElement.selectedIndex];

        inputDropTo = $(selectedElement).attr('accesskey');
        toFullAddress = $(selectedElement).attr('id');
    }
    else
        toFullAddress = inputDropTo = $('#textarea-drop-to').val();

    if (!inputPickFrom.length) {
        highlight($('#input-pick-from, #textarea-pick-from').focus());
        return;
    }
    if (!inputDropTo.length) {
        highlight($('#input-drop-to, #textarea-drop-to').focus());
        return;
    }

    if (fromFullAddress == toFullAddress) {

        $.Zebra_Dialog('It seems that you entered invalid locations.', {
            'type': 'error',
            'title': 'Invalid Pickup/Dropoff Locations!'
        });

        return;
    }

    if ($('.diversions-and-stops .selected').html() == 'Yes') {
        if (!$('#hidden-stops').val().length) {
            highlight($('#stops').parent().focus());

            $.Zebra_Dialog('You must enter at least one diversion/stop.<br /><br />Otherwise, please uncheck <strong>"Diversions & Stops"</strong> option.', {
                'type': 'error',
                'title': 'No via stop added!'
            });

            return;
        }
    }

    loadSpinner('html');

    var data =
	{
	    pickFrom: fromFullAddress,
	    dropTo: toFullAddress,
	    selected: $('#pick-from-type').val() + '----' + $('#drop-to-type').val(),
	    journeyType: $('#journey-type').val(),
	    fromPostCode: inputPickFrom,
	    toPostCode: inputDropTo
	};

    $('.mainbanner').slideUp();
	$('.Maincontant').load('/Main/SelectTaxi', data, function () {
	    $('.spinner').hide();
	});
}

var postCodeElement = $('#postal-codes');

function postalCodesDropDown(target, parent) {
    var data = { term: target.value };

    $(target).parent().find('#postcodes-hr').show();
    var postCodesElement = $(target).parent().find('#postcodes').html('<option id>---- select your address ----</option>').show()[0];

    loadSpinner(postCodesElement);

    $.post('/Address/Search', data, function (response) {
        $('.spinner').hide();

        var container = $(parent);
        if (target.id == 'to-postcode') container = $(target).parent().parent().parent();

        var topSide = 0, leftSide = 0;
        if ($(container).length) {
            topSide = $(container).offset().top;
            leftSide = $(container).offset().left;
        }

        var length = response.length, code;

        for (var i = 0; i < length; i++) {
            code = response[i];
            postCodesElement.innerHTML += '<option id="' + code.d + '">' + code.p + '</option>';
        }

        $(postCodesElement).find('option').click(function () {
            target.value = $(this).attr('accesskey') || $(this).html();
            $(postCodeElement).hide();
        });
    });
}

function addressesHandling(element) {
    showTextElement(element, '');
    $(element).find('textarea, input, .options-list, .search-uk-addresses').hide();

    $(element).find('#textarea-pick-from, #textarea-drop-to, #textarea-stops').show().focus().geo_autocomplete(new google.maps.Geocoder, {
        mapkey: 'ABQIAAAAbnvDoAoYOSW2iqoXiGTpYBTIx7cuHpcaq3fYV4NM0BaZl8OxDxS9pQpgJkMv0RxjVl6cDGhDNERjaQ',
        selectFirst: false,
        minChars: 3,
        cacheLength: 50,
        width: 300,
        scroll: true,
        scrollHeight: 330
    });
}

var target;

function page1() {
    $('#get-quote').click(getQuoteClick);
    $('#pick-from-type, #drop-to-type, #stops-type').change(function () {
        var option = $(this).val(), element = $(this).parent().parent();

        option === 'Post code' && postalCodesHandling(element);
        option === 'Airport' && airportsHandling(element);
        option === 'Address' && addressesHandling(element);
        option === 'Hotel' && hotelsHandling(element);
        option === 'Train Station' && trainStationsHandling(element);
        option === 'Famous Place' && famousPlacesHandling(element);
    });

    $('#input-pick-from,#input-drop-to, #input-stops').focus(function () {
        target = this;
        searchPostcodes();
    });

    
    $('.search-uk-addresses').click(function () {
        target = $(this).parent().find('#input-pick-from,#input-drop-to,#input-stops')[0];
        searchPostcodes();
    });

    function searchPostcodes() {
        $.Zebra_Dialog('<input type="text" id="search-postcode" style="width:88%" value="' + target.value + '"><div id="output-postcodes" class="dropdown output-postcodes"></div><div id="search-postcode-button" class="blue-button" style="margin: 0;background-color: #3BD53B;border-radius: 0;">Search</div><div id="close-postcode-box" class="blue-button" style="margin: 0;margin-left:-1px;border-radius: 0;margin-top:5px;">Close</div>', {
            'custom_class': 'search-image',
            'buttons': false
        });
        $('#search-uk-addresses, #search-postcode-button').click(search);
        $('#close-postcode-box').click(function() {
            $('.ZebraDialog, .ZebraDialogOverlay, #output-postcodes').html('').hide();
        });
        
        function search() {
            
            $('#output-postcodes').html('<span style="padding: 5px 10px;display:block;text-align: left;font-size: 20px;">Please wait...</span>').slideDown();

            var data = { term: $('#search-postcode').val() };

            $.post('/Address/Search', data, function (response) {

                var length = response.length, code, html = '';

                if (length == 0) {
                    html += '<blockquote>It seems that current postcode not exists in our database. Kindly, please enter valid postcode and street so we can update our database.</blockquote>';
                    html += '<blockquote><label style="color:black;width:85px;">Postcode: </label> <input type=text id=postcode-text value="' + $('#search-postcode').val() + '"><br />';
                    html += '<label style="color:black;width:85px;">Street: </label> <input type=text id=street-text autofocus></blockquote>';
                    html += '<blockquote id="select-postcode-street" class="blue-button" style="background-color: #14F3CF;border-radius: 0;margin: -13px 47px 8px 47px">Select Postcode/Street</blockquote>';
                    $('#output-postcodes').html(html).slideDown();

                    $('#select-postcode-street').click(function () {
                        var elem = getEmptyElement('postcode-text, street-text');
                        if (elem !== true) return;

                        var postcode = $('#postcode-text').val(),
                            street = $('#street-text').val();

                        target.value = postcode + ', ' + street;
                        $(target).attr('accesskey', postcode);

                        $('.ZebraDialog, .ZebraDialogOverlay, #output-postcodes').html('').hide();

                        $.post('/Address/NoPostcode', { postcode: postcode, street: street });
                    });
                }
                else {
                    for (var i = 0; i < length; i++) {
                        code = response[i];
                        html += '<div id="' + code.d + '" accesskey="'+ (code.s ? '' : 'no-street') +'">' + code.p + (code.s ? ', ' + code.s : '') + '</div>';
                    }
                    $('#output-postcodes').html(html).slideDown();
                }

                $('#output-postcodes div').click(function () {
                    var html_value = $(this).html();
                    if($(this).attr('accesskey') == 'no-street')
                    {
                        html = '';
                        html += '<blockquote>It seems that current postcode exists in our database but street not found. Kindly, please enter valid street so we can update our database.</blockquote>';
                        html += '<blockquote><label style="color:black;width:85px;">Postcode: </label> <input type=text id=postcode-text value="' + html_value + '"><br />';
                        html += '<label style="color:black;width:85px;">Street: </label> <input type=text id=street-text autofocus></blockquote>';
                        html += '<blockquote id="select-postcode-street" class="blue-button" style="background-color: #14F3CF;border-radius: 0;margin: -13px 47px 8px 47px">Select Postcode/Street</blockquote>';
                        $('#output-postcodes').html(html).slideDown();

                        $('#select-postcode-street').click(function () {
                            var elem = getEmptyElement('postcode-text, street-text');
                            if (elem !== true) return;

                            var postcode = $('#postcode-text').val(),
                                street = $('#street-text').val();

                            target.value = postcode + ', ' + street;
                            $(target).attr('accesskey', postcode);

                            $('.ZebraDialog, .ZebraDialogOverlay, #output-postcodes').html('').hide();

                            $.post('/Address/NoPostcode', { postcode: postcode, street: street });
                        });
                    }
                    else
                    {
                        target.value = $(this).html();
                        $(target).attr('accesskey', $(this).attr('id'));

                        $('.ZebraDialog, .ZebraDialogOverlay, #output-postcodes').html('').hide();
                    }
                });
            });
        }
    }
}
page1();

var prevPageIndex = 1;
$('.back-to-booking').click(function () {
    $('.page-' + (prevPageIndex + 1)).css('height', 'auto').slideUp();
    $('.page-' + prevPageIndex).css('height', 'auto').slideDown();

    if (prevPageIndex == 1) $('#vehicles-nav').removeClass('done');
    if (prevPageIndex == 2) $('#details-nav').removeClass('done');
    if (prevPageIndex == 3) $('#final-nav').removeClass('done');

    prevPageIndex -= 1;

    if(prevPageIndex <= 0) prevPageIndex = 1;
});