document.getElementById("delivery-btn").addEventListener("click", openPopup);
function openPopup() {
    document.getElementById("popup").style.display = "block";
    document.getElementById("delivery-btn").style.display = "none";
    document.getElementById("done-btn").style.display = "block";
}   
function closePopup() {
    document.getElementById("popup").style.display = "none";
    document.getElementById("delivery-btn").style.display = "block";
    document.getElementById("done-btn").style.display = "none";
}
