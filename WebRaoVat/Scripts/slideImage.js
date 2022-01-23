// next and prev
function plusSlides(n) {
    showSlides(slideIndex += n);
    clearInterval(timer);
    timer = setInterval(plusSlides, 5000, 1);
}
var slideIndex = 1;
var slides = document.getElementsByClassName("image-slide");
showSlides(slideIndex);
function showSlides(n) {
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (var i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    slides[slideIndex - 1].style.display = "block";
}
//auto slide
var timer = setInterval(plusSlides, 5000, 1);
//hover to stop interval
window.onload = function () {
    slides.onmouseover = function () { clearInterval(timer); };
    slides.onmouseout = function () { timer = setInterval(plusSlides, 5000, 1); };
}