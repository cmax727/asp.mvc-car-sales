function highlight(elem) {
    $(elem).focus().toggleClass('highlight');
    var counter = 0,
        interval = setInterval(function () {
            $(elem).toggleClass('highlight');
            counter++;
            if (counter > 6) clearInterval(interval);
        }, 100);
}
function getEmptyElement(arg)
{
    var ids = arg.split(', '), length = ids.length;
    for (var i = 0; i < length; i++)
    {
        var elem = $('#' + ids[i])[0];
        if (elem && !elem.value.length)
        {
            highlight(elem);
            return elem;
        }
    }
    return true;
}

function validate(value) {
    value = value.replace(/-/g, '__').replace(/\?/g, '-qmark').replace(/ /g, '--').replace(/\n/g, '-n').replace(/</g, '-lt').replace(/>/g, '-gt').replace(/&/g, '-amp').replace(/#/g, '-nsign').replace(/__t-n/g, '__t').replace(/\+/g, '_plus_').replace(/=/g, '-equal');
    return value;
}

function loadSpinner(parent) {
    var left, top, width, height, tempParent = parent;

    if (parent == 'html') parent = document.documentElement;
    else parent = $(parent)[0];

    if (!parent) return;

    left = parent.offsetLeft;
    top = parent.offsetTop;

    if (tempParent == 'html') parent = document.body;
    width = parent.clientWidth;
    height = parent.clientHeight;

    $('.spinner').css('left', left).css('top', top).css('height', height).css('width', width).fadeIn();
}

function sticky(element, fixedTop, absoluteTop) {
    $(window).scroll(function () {
        var x = window.scrollY;
        if (x > 100) $(element).css('top', fixedTop).css('position', 'fixed');
        else $(element).css('top', absoluteTop).css('position', 'absolute');
    });
}

sticky('.menu-container', 0, 'auto');

function loadLoginHTML(callback) {
    if (!$('#login-section').length) {
        $.get('/Account/Login', function (html) {
            $('#login-section-container').html(html);
        });
        callback && callback();
    }
    else callback && callback();
}
function handleMenu()
{
    $('.client-menu').fadeIn();
    $('.login').click(function () {
        $('#signup-section').fadeOut();
        loadLoginHTML(function () {
            loadSpinner('html');
            $('#login-section').slideDown().find('#login-email').focus();
        });
    });

    $('.login, .logout').mouseenter(function () {
        $('.quick-menu').show();
    });
    $('.quick-menu, .quick-menu *').mouseenter(function () {
        $('.quick-menu').show();
    });

    $('.header-container, .menu-container, .cover-full-screen').mouseenter(function() {
        $('.quick-menu').hide();
    });

    $('.logout').click(function (e) {
        if (e.target.className != 'login-small-icon') {
            loadSpinner('html');

            var data = {
                __RequestVerificationToken: $('#logout-anti-forgery').find('[name="__RequestVerificationToken"]').val()
            };

            $.post('/Account/Logout', data, function (response) {
                if (response == true) window.location = '/';
            });
        }
        else $('.quick-menu').fadeIn();
    });
    
    $(document).click(function (e) {
        if (e.target.className !== 'login-small-icon') $('.quick-menu').fadeOut();
    });
}

function call() {
    var length = arguments.length;
    for (var i = 0; i < length; i++) {
        arguments[i]();
    }
}

function removeDots(value) {
    if ((value + '').indexOf('.') !== -1)
        value = (value + '').substr(0, (value + '').indexOf('.') + 2);

    return value;
}

window.onload = function () {
    openAccountClickHandling();
    staffSignup();
};

function onQuickLoad() {
    handleMenu();
    menuHovers();
}


function menuHovers() {
    var visibleSubMenu = '';
    $('.menu-section a').mouseover(function () {
        visibleSubMenu = '.sub-menu[id="' + $(this).attr('id') + '"]';
        $('.sub-menu').hide();
        $(visibleSubMenu).show();
    });

    $('.menu-container, .header-container').mousemove(function () {
        var main = $('.main-content')[0];
       
        $('.cover-full-screen').fadeIn(5).animate({
            top: main.offsetTop,
            height: main.clientHeight
        }, 20);
    });

    $('.cover-full-screen').mouseover(function () {
        $(this).fadeOut();
        visibleSubMenu = '';
        $('.sub-menu').hide();
    });

    $('.menu-section, .sub-menu').mouseover(function () {
        $(visibleSubMenu).show();
    });

    $('#airport-transfers, #airport-transfers-sub a').mouseenter(function () {
        $('#airport-transfers-sub').show();
    }).mouseout(function () {
        $('#airport-transfers-sub').hide();
    });
}

function openAccountClickHandling() {
    $('.open-an-account, .signup').click(function () {
        $('#login-section').fadeOut();
        var element = this;
        loadSpinner('html');

        if ($(element).find('a').length) {
            window.location = $(element).find('a').attr('href');
            return;
        }


        if ($('.logout').length) {
            alertBox('You already logged in!');
            return;
        }

        $.get('/Account/NewClientAccount', function (html) {
            $('.spinner').hide();
            $('.about-content').html(html).find('#signup-section').fadeIn().css('box-shadow', '0 0 5px black').find('#signup-button').css('margin-top', 10).css('margin-left', 185).parent().find('#signup-email').focus(); ;
        });
    });
}

function staffSignup() {
    $('.staff-signup').click(function () {
        alertBox('Access is denied.');
    });
}

function alertBox(message, callback) {
    loadSpinner('html');
    setTimeout(function () {
        if (!callback) alert(message);
        else callback();

        $('.spinner').fadeOut();
    }, 400);
}

function toolTip() {
    $('.Zebra_Tooltip').hide();
    $(document).ready(function () {
        new $.Zebra_Tooltips($('.tip'));
    });
}

toolTip();