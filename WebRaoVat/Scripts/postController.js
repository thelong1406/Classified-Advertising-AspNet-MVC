var item = {
    post_id: 2,
    title: "test",
    content: "test",
    date_posted: Date.now(),
    user_id: "admin",
    price: 50000,
    is_hided: true,
    category_id: 1,
    is_approved: true,
    is_seller: true,
    ward_id: 1,
    product_id: 1
};
var post = {
    init: function () {
        post.loadCate();
        post.loadSecondCate();
        post.postEvents();
    },
    loadCate: function () {
        $.ajax({
            url: "/Post/Category",
            type: "POST",
            dataType: "html",
            success: function (data) {
                $('#Display').html(data);
            }
        });
    },
    loadSecondCate: function loadSecond() {
        $('#cate').on('click', function (e) {
            e.preventDefault();

            var btn = $(this);
            var id = btn.data('id');
            item.category_id = id;
            $.ajax({
                url: "/Post/Second_Cate",
                data: { id: id },
                type: "POST",
                dataType: "html",
                success: function (data) {
                    $('#Display').html(data);
                }
            });
        });
    },

    postEvents: function () {



        $('#Post').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                url: "/Post/Post",
                type: "POST",
                data: item,
                dataType: "json",
                success: function (responce) {
                    console.log(responce);
                    alert("hi");
                },
                error: function (result) {
                    alert(result.result);
                }
            });
        });

    }
}
post.init();


