function CopyCurrentURL()
    {
        var dummy = document.createElement('input'),
          text = window.location.href;
        document.body.appendChild(dummy);
        dummy.value = text;
        dummy.select();
        document.execCommand('copy');
    document.body.removeChild(dummy);
    alert("This URL have been copy!");
    }