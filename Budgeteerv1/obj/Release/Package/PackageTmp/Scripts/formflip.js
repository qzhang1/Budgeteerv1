document.getElementById('editform').addEventListener('click', function (event) {

    event.preventDefault();
    $('#lameHack').stop(true, true).delay(1000).animate({ height: ($('#side-2').height() + 60) + 'px' }, 500);
    $('#side-2').attr('class','flip flip-side-1');
    $('#side-1').attr('class', 'flip flip-side-2');

}, false);

document.getElementById('createform').addEventListener('click', function (event) {

    event.preventDefault();
    $('#lameHack').stop(true, true).delay(0).animate({ height: ($('#side-1').height() + 60) + 'px' }, 900);
    $('#side-1').attr('class', 'flip');
    $('#side-2').attr('class', 'flip');

}, false);

$('#lameHack').height($('#side-1').height() + 20);