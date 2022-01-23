function showListDown() {
    var list = document.getElementsByClassName("list-down");
    var div = document.getElementById("have-list-down")
    for (var i = 0; i < list.length; i++) {
        if (list[i].style.display == "block") {
            div.style.display = "none";
            list[i].style.display = "none";
        }

        else
        {
            div.style.display = "block";
            list[i].style.display = "block";
        }
    }
}
