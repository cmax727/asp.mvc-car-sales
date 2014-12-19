var editButton = $('.edit-button'),
    editBox = $('#edit-box'),
    isEditing,
    targetEdit, oldValue;

$('.account-info span').mouseenter(function () {
    if (isEditing) return;

    $(editButton).css('top', this.offsetTop + 3).css('left', this.offsetLeft + this.clientWidth - 70).show();
    targetEdit = this;
}).mouseout(function () {
    $(editButton).hide();
});

$(editButton).mouseenter(function () {
    $(this).show();
}).click(function () {
    if (!targetEdit) return;

    if ($(targetEdit).is('.latest-job, .job-div')) {
        showEditJobPanel();
    }
    else {
        $(editBox).css('padding-left', '18px').css('top', targetEdit.offsetTop).css('left', targetEdit.offsetLeft).show().css('width', targetEdit.clientWidth - 30).css('height', targetEdit.clientHeight - 10).val(targetEdit.innerHTML).focus();
        oldValue = $(editBox).val();
    }

    isEditing = true;
    $(this).hide();
});

function showEditJobPanel(id) {
    //location = '/Client/Edit?id=' + id;
    $.get('/Client/TempJobEdit', { id: id }, function (html) {
        $('.spinner').hide();
        $('.about-content').html(html);
        $('.client-left-menu').animate({
            width: 0,
            opacity:0
        });
    });
}

$(editBox).blur(function () {
    if (!targetEdit) return;

    var value = this.value,
        field = $(targetEdit).parent().attr('accesskey');

    loadSpinner('.account-info');
    $.post('/Client/UpdateAccount?field=' + field + '&value=' + value, function (response) {
        $(editBox).fadeOut();
        $('.spinner').fadeOut();

        if (response == false) {
            $(targetEdit).html(oldValue || '');
            highlight(targetEdit);
        }
        else $(targetEdit).html(value);

        isEditing = targetEdit = null;
    });
});

$('a.left-a').click(function () {
    $('li.active').removeClass('active');
    $(this).parent().addClass('active');

    isEditing = targetEdit = null;
    $(editButton).hide();
    $(manageNotification).hide();

    $('.latest-job, .account-info, .jobs-history, .notifications').fadeOut(10);
    var id = this.id;

    if (id == 'home') location = '/';
    if (id == 'feedback') {
        //location = '/Feedback/';
        $.Zebra_Dialog('Feedback is under construction.', { 'type': 'information' });
    }

    if (id == 'latest') getLatestJob(this);
    if (id == 'history') {
        $('.jobs-history').html('');
        getClientJobs(this);
    }
    if (id == 'account') $('.account-info').fadeIn();
    if (id == "notifications") {
        $('.notifications').fadeIn();
        setTimeout(function () {
            var seleted = $('.top-menu div.selected'),
                id = seleted.attr('id');
            handleNotificationMenuClick(id, seleted);
        }, 100);
    }
});

$('#make-a-booking').click(function () {
    loadSpinner('html');
    window.location = '/book';
});

$('.top-menu div').click(function () {
    $(manageNotification).hide();

    $('.top-menu .selected').removeClass('selected');
    $(this).addClass('selected').find('.select').addClass('selected');

    $('.notifications-for-you, .messages-by-you, .new-message').fadeOut(10);

    var id = this.id;
    handleNotificationMenuClick(id, this);
});

function handleNotificationMenuClick(id, _this) {
    if (id == 'notification') getClientNotifications(_this);
    if (id == 'messages') getClientMessages(_this);
    if (id == 'new-message') $('.new-message').fadeIn();
}


$('#send-direct-message').click(function () {
    if (!$('#direct-message').val().length) {
        highlight('#direct-message');
        return;
    }

    var to = $('#message-to').val(),
        type = $('#message-type').val(),
        message = validate($('#direct-message').val());

    loadSpinner('.new-message');
    $.post('/Client/DirectMessage', {
        to: to,
        type: type,
        message: message
    }, function (response) {
        if (response == true) {
            $('#direct-message').val('');

            $('.top-menu .selected').removeClass('selected');
            $('#messages').addClass('selected').find('.select').addClass('selected');
            getClientMessages();
            $('.new-message').fadeOut();
        }
    });
});

function getLatestJob(menu) {
    loadSpinner('.client-left-menu');

    $.post('/Client/GetLatestJob', function (html) {
        $('.spinner').fadeOut()
        $('#jobs-container').html(html).find('.latest-job').fadeIn();
        showEditButtonForJob();
    });
}

var skip = 0, take = 10, jobsCount = 0;
function getClientJobs(menu) {
    loadSpinner('.client-left-menu');

    $.post('/Client/GetClientJobs', { skip: skip, take: take }, function (html) {
        $('.spinner').fadeOut()

        var prevHTML = $('.jobs-history').html();
        $('.jobs-history').html(prevHTML + html).fadeIn()

        .find('#show-more').click(function () {
            var self = this;
            this.parentNode.removeChild(this);

            take += 10; skip += 10;
            if (take <= jobsCount) {
                $.post('/Client/GetClientJobs', { skip: skip, take: take }, function (response) {
                    prevHTML = $('.jobs-history').html();

                    $('.jobs-history').html(prevHTML + response).append(self);
                    clientJobsTRClicks();
                });
            }
        });
        clientJobsTRClicks();
    });
}

var prevPage;
function clientJobsTRClicks() {
    $('.jobs-list tbody tr').click(function () {
        loadSpinner('.jobs-history');

        var element = this;

        var data = {
            token: $(element).attr('id')
        };

        $.post('/Client/ExpandBooking', data, function (html) {
            $('.spinner').hide();
            prevPage = $('.jobs-history').html();
            $('.jobs-history').html(html);
            window.scrollTo(0, 0);
        });
    });
}

function showEditButtonForJob() {
    $('.latest-job, .job-div').mouseenter(function () {
        if (!$(this).is('.allow-edit')) return;
        
        $(editButton).css('top', this.offsetTop + 3).css('left', this.offsetLeft + this.clientWidth - 70).show();
        targetEdit = this;
    });
}

function getClientNotifications(_this) {
    loadSpinner(_this);

    $.post('/Client/GetClientNotifications', function (html) {
        $('.spinner').fadeOut()
        $('.notifications-for-you').html(html).fadeIn();

        notifictionManagement();
    });
}

function getClientMessages(_this) {
    loadSpinner(_this);

    $.post('/Client/GetClientMessages', function (html) {
        $('.spinner').fadeOut();
        $('.messages-by-you').html(html).fadeIn();
    });
}

var isManagingNotification, notificationDiv, manageNotification = $('.manage-notification');
function notifictionManagement() {
    $('.your-notification').mouseenter(function () {
        if (isManagingNotification) return;

        $(manageNotification).css('top', this.offsetTop + 4).css('left', this.offsetLeft + this.clientWidth - 118).show();

        if ($(this).attr('accesskey') == 'New') {
            $('.notification-management-options #mark-as-unread').attr('id', 'mark-as-read').html('Mark as read');
        }

        if ($(this).attr('accesskey') == 'Read') {
            $('.notification-management-options #mark-as-read').attr('id', 'mark-as-unread').html('Mark as unread');
        }

        notificationDiv = this;
    }).mousemove(function () {
        $('.notification-management-options').hide();
    });
}

$(manageNotification).click(function () {
    if (!isManagingNotification) return;
}).mouseenter(function () {
    $('.notification-management-options').css('top', this.offsetTop + this.clientHeight).css('left', this.offsetLeft).show();
})

$('.notification-management-options div').mouseenter(function () {
    $('.notification-management-options').show();
}).mouseout(function () {
    $('.notification-management-options').hide();
});

$('.notification-management-options div').click(function () {
    var id = this.id;

    if (id == 'mark-as-read') {
        $(this).attr('id', 'mark-as-unread').html('Mark as unread');

        loadSpinner($(this).parent());
        updateNotification('Read', function (response) {
            if (response == true) {
                $(notificationDiv).attr('accesskey', 'Read').find('.roshan').html('Read');
                $(notificationDiv).find('.latest-job').slideUp();
            }
            $('.spinner').fadeOut();
        });
    }
    else if (id == 'mark-as-unread') {
        $(this).attr('id', 'mark-as-read').html('Mark as read');

        loadSpinner($(this).parent());
        updateNotification('New', function (response) {
            if (response == true) {
                $(notificationDiv).attr('accesskey', 'New').find('.roshan').html('New');
                $(notificationDiv).find('.latest-job').fadeIn(1000);
            }
            $('.spinner').fadeOut();
        });
    }

});

function updateNotification(status, callback) {
    var id = $(notificationDiv).attr('id');

    var data = {
        token: id,
        status: status
    };

    $.post('/Client/UpdateNotification', data, function (respose) {
        callback(respose);
    });
}