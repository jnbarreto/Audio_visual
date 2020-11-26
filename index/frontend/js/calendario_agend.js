mobiscroll.settings = {
    lang: 'pt-BR',                           // Specify language like: lang: 'pl' or omit setting to use default
    theme: 'windows',                        // Specify theme like: theme: 'ios' or omit setting to use default
    themeVariant: 'light'                    // More info about themeVariant: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#opt-themeVariant
};

var monthInst,
    dayInst,
    popupInst,
    dateInst,
    preventSet,
    allDaySwitch = document.getElementById('allDay'),
    eventTextInput = document.getElementById('eventText'),
    eventDescInput = document.getElementById('eventDesc'),
    now = new Date(),
    //btn = '<button class="mbsc-btn mbsc-btn-outline mbsc-btn-danger md-delete-btn mbsc-ios">Delete</button>',
    myData = [{
        start: new Date(now.getFullYear(), now.getMonth(), 9, 13),
        end: new Date(now.getFullYear(), now.getMonth(), 9, 13, 30),
        text: 'Lunch @ Butcher\'s',
        color: '#26c57d'
    }, {
        start: new Date(now.getFullYear(), now.getMonth(), now.getDate(), 15),
        end: new Date(now.getFullYear(), now.getMonth(), now.getDate(), 16),
        text: 'General orientation' ,
        color: '#fd966a'
    }, {
        start: new Date(now.getFullYear(), now.getMonth(), now.getDate() - 1, 18),
        end: new Date(now.getFullYear(), now.getMonth(), now.getDate() - 1, 22),
        text: 'Dexter BD',
        color: '#37bbe4'
    }, {
        start: new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1, 10, 30),
        end: new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1, 11, 30),
        text: 'Stakeholder mtg.',
        color: '#d00f0f'
    }];

function navigate(inst, val) {
    if (inst) {
        inst.navigate(val);
    }
}

dateInst = mobiscroll.range('#eventDate', {
    controls: ['date', 'time'],
    dateWheels: '|D M d|',
    endInput: '#endInput',
    tabs: false,
    responsive: {                            // More info about responsive: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#opt-responsive
        large: {
            touchUi: false
        }
    },
    cssClass: 'md-add-event-range'
});

dayInst = mobiscroll.eventcalendar('#demo-add-event-day', {
    display: 'inline',                       // Specify display mode like: display: 'bottom' or omit setting to use default
    view: {                                  // More info about view: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#opt-view
        eventList: { type: 'day' }
    },
    data: myData,                            // More info about data: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#opt-data
    onPageChange: function (event, inst) {   // More info about onPageChange: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#event-onPageChange
        var day = event.firstDay;
        preventSet = true;
        navigate(monthInst, day);
        dateInst.setVal([day, new Date(day.getFullYear(), day.getMonth(), day.getDate(), day.getHours() + 2)], true);
    },
    onEventSelect: function (event, inst) {  // More info about onEventSelect: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#event-onEventSelect
        if (event.domEvent.target.classList.contains('md-delete-btn')) {
            mobiscroll.confirm({ 
                
                title: 'Confirm Deletion',
                message: 'Are you sure you want to delete this item?',
                okText: 'Delete',
                callback: function (res) {
                    if (res) {
                        inst.removeEvent([event.event._id]);
                        monthInst.removeEvent([event.event._id]);
                        mobiscroll.toast({ 
                            
                            message: 'Deleted'
                        });
                    }
                }
            });
        }
    }
});

monthInst = mobiscroll.eventcalendar('#demo-add-event-month', {
    display: 'inline',                       // Specify display mode like: display: 'bottom' or omit setting to use default
    view: {                                  // More info about view: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#opt-view
        calendar: { type: 'month' }
    },
    data: myData,                            // More info about data: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#opt-data
    onSetDate: function (event, inst) {      // More info about onSetDate: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#event-onSetDate
        if (!preventSet) {
            var day = event.date;
            navigate(dayInst, day);
            dateInst.setVal([day, new Date(day.getFullYear(), day.getMonth(), day.getDate(), day.getHours() + 2)], true);
        }
        preventSet = false;
    }
});

document
    .getElementById('allDay')
    .addEventListener('change', function () {
        var allDay = this.checked;

        dateInst.option({
            controls: allDay ? ['date'] : ['date', 'time'],
            dateWheels: (allDay ? 'MM dd yy' : '|D M d|')
        });
    });

popupInst = mobiscroll.popup('#demo-add-event-popup', {
    display: 'center',                       // Specify display mode like: display: 'bottom' or omit setting to use default
    cssClass: 'mbsc-no-padding',
    buttons: [{                              // More info about buttons: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#opt-buttons
            text: 'Add event',
            handler: 'set'
        },
        'cancel'
    ],
    headerText: 'Add new event',             // More info about headerText: https://docs.mobiscroll.com/4-10-3/javascript/eventcalendar#opt-headerText
    onSet: function (event, inst) {
        var eventDates = dateInst.getVal(),
            events = {
                start: eventDates[0],
                end: eventDates[1],
                text: (eventTextInput.value || 'New Event') + btn,
                title: eventTextInput.value || 'New Event',
                description: eventDescInput.value,
                allDay: allDaySwitch.checked,
                free: document.querySelector('input[name="free"]:checked').value == 'true'
            };
        monthInst.addEvent(events);
        dayInst.addEvent(events);
        eventTextInput.value = '';
        eventDescInput.value = '';
        // Navigate the calendar to the new event's start date
        monthInst.navigate(eventDates[0], true);
    }
});

document
    .getElementById('showAddEventPopup')
    .addEventListener('click', function () {
        popupInst.show();
    }, false);




function removeTrial()
{
    var obj = jQuery(".mbsc-cal-day-i");
    // var conteste = 0;
    // for(var i = 0; i < obj.length; i++)
    // {
    //     if(obj[i].childNodes.length > 1)
    //     {
    //         jQuery(obj[i].childNodes[1]).remove();

    //         console.log(conteste + "item removido");
    //         conteste++
    //     }
    // }


    for(var i = 0; i < obj.length; i++)
    {
        jQuery(obj[i]).attr("id", "calendar"+i);
    }

    for (var o = 0; o < obj.length; o++) 
    {
        if(jQuery("#calendar"+o)[0].childNodes != undefined)
        {
            if(jQuery("#calendar"+o)[0].childNodes.length > 1)
            {
                jQuery("#calendar"+o)[0].childNodes[1].innerHTML = "";
                jQuery("#calendar"+o)[0].childNodes[1].innerText = "";

                jQuery("#calendar"+o)[0].childNodes[1].remove();
            }
        }
    }
}    