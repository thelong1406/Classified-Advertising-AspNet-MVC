var check = false;
function openRating() {
    document.getElementById("rating-panel").style.display = "block";
}
function closeRating() {
    document.getElementById("rating-panel").style.display = "none";
}
function showRateButton() {
    if (check == false) {
        $(document).ready(function () {
            $('#container').append('<input class="theme-btn bg-hong noselect" value="Send Rating" type="submit" name="submit" id="submit" />');
        });
        check = true;
    }
}