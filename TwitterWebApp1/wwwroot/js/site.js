// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('textarea[maxlength]').each(function () {
    this.setAttribute("style", "height:" + (this.scrollHeight) + "px;overflow-y:hidden;");
}).on("input", function () {
    this.style.height = 0;
    this.style.height = (this.scrollHeight) + "px";
});

// this initializes the maxlength plugin for all textareas with the maxlength attribute
$('textarea[maxlength]').maxlength({
    alwaysShow: true,
    appendToParent: true,
    warningClass: "form-text text-muted mt-4",
    limitReachedClass: "form-text text-muted mt-4",
    placement: 'top-right-inside',
});

$(document).ready(function () {
    $('#createpost-imgfile').on('change', function (e) {
        displaySelectedImage(e, 'createpost-imgpreview');
        $('.uploaded-media').show();
    });
});

$(document).ready(function () {
    $('.uploaded-media-remove button').click(function () {
        $('#createpost-imgfile').val('');
        $('#createpost-imgpreview').attr('src', '');
        $('.uploaded-media').hide();
    });
});

$(document).ready(function () {
    $('#pp-imgfile').on('change', function (e) {
        displaySelectedImage(e, 'pp-imgpreview');
    });
});

$(document).ready(function () {
    $(document).on('click', '.like-btn', function () {
        var hashCode = $(this).data('hashcode');
        var requestUrl = $(this).data('requesturl');
        $.ajax({
            type: 'POST',
            url: requestUrl,
            dataType: 'json',
            data: { hash: hashCode },
            success: function (response) {
                // Update the likes count without reloading the page
                $("#like-count-" + hashCode).text(response.likesCount);
            },
            error: function (req, status, error) {
                alert("An error occurred while liking the post." + req.responseText);
            },
        });
    });
});

$(document).ready(function () {
    $(document).on('click', '.bookmark-btn', function () {
        var hashCode = $(this).data('hashcode');
        var requestUrl = $(this).data('requesturl');
        console.log('clicked');
        $.ajax({
            type: 'POST',
            url: requestUrl,
            dataType: 'json',
            data: { hash: hashCode },
            success: function (response) {
                // Update the bookmarks count without reloading the page
                $("#bookmark-count-" + hashCode).text(response.bookmarksCount);
            },
            error: function (req, status, error) {
                alert("An error occurred while bookmarking the post." + req.responseText);
            },
        });
    });
});

// this updates the image preview when a new image is selected
function displaySelectedImage(event, elementId) {
    const selectedImage = document.getElementById(elementId);
    const fileInput = event.target;
    const maxAllowedSize = 5 * 1024 * 1024; // 5MB

    if (fileInput.files && fileInput.files[0]) {
        if (fileInput.files[0].size > maxAllowedSize) {
            alert("Image size is too large. Please select an image less than 10MB.");
            return;
        }

        const reader = new FileReader();

        reader.onload = function (e) {
            selectedImage.src = e.target.result;
        };

        reader.readAsDataURL(fileInput.files[0]);
    }
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

$(document).ready(function () {

    if (window.location.pathname !== '/Home/Index'
        && window.location.pathname !== '/Home'
        && window.location.pathname !== '/') {
        return;
    }

    var skip = 1;
    var isLoading = false;
    loadPosts();

    $('.feed').scroll(function () {
        if ($('.feed').scrollTop() + $('.feed').height() >= $('#posts-container').height() + 174) {
            if (!isLoading) {
                loadPosts(1000);
            }
        }
    });

    function loadPosts(delay = 0) {
        isLoading = true;
        const loadingIndicator = '<div class="loading-indicator text-center mt-3"><i class="fa-solid fa-spinner fa-spin fa-2x"></i></div>';

        $('#posts-container').append(loadingIndicator);

        setTimeout(function () {
            $.ajax({
                url: '/Home/FetchPosts',
                type: 'GET',
                data: { skipPost: skip },
                success: function (data) {
                    if (data.length > 0) {
                        $('#posts-container').append(data);
                        skip++;
                    }
                    $('#posts-container .loading-indicator').remove();
                    isLoading = false;
                },
                error: function () {
                    console.log('Error loading posts.');
                    isLoading = false;
                }
            });
        }, delay);
    }
});

$(document).ready(function () {

    if (window.location.pathname !== '/Profile/Post') {
        return;
    }

    var skip = 1;
    var isLoading = false;
    loadPosts();

    $('.feed').scroll(function () {
        if ($('.feed').scrollTop() + $('.feed').height() >= $('#reply-posts-container').height() + 174) {
            if (!isLoading) {
                loadPosts(1000);
            }
        }
    });

    function loadPosts(delay = 0) {
        isLoading = true;
        const loadingIndicator = '<div class="loading-indicator text-center mt-3"><i class="fa-solid fa-spinner fa-spin fa-2x"></i></div>';

        $('#reply-posts-container').append(loadingIndicator);

        const urlParams = new URLSearchParams(window.location.search);
        const postHashCode = urlParams.get('postHashCode');

        setTimeout(function () {
            $.ajax({
                url: '/Profile/FetchReplyPosts',
                type: 'GET',
                data: {
                    skipReplyPost: skip,
                    postHashCode: postHashCode,
                },
                success: function (data) {
                    if (data.length > 0) {
                        $('#reply-posts-container').append(data);
                        skip++;
                    }
                    $('#reply-posts-container .loading-indicator').remove();
                    isLoading = false;
                },
                error: function () {
                    console.log('Error loading replyposts.');
                    isLoading = false;
                }
            });
        }, delay);
    }
});

$(document).ready(function () {

    if (window.location.pathname !== '/Profile') {
        return;
    }

    var skip = 1;
    var isLoading = false;
    loadPosts();

    $('.feed').scroll(function () {
        if ($('.feed').scrollTop() + $('.feed').height() >= $('#profile-posts-container').height() + 174) {
            if (!isLoading) {
                loadPosts(1000);
            }
        }
    });

    function loadPosts(delay = 0) {
        isLoading = true;
        const loadingIndicator = '<div class="loading-indicator text-center mt-3"><i class="fa-solid fa-spinner fa-spin fa-2x"></i></div>';

        $('#profile-posts-container').append(loadingIndicator);

        const urlParams = new URLSearchParams(window.location.search);
        const username = urlParams.get('username');

        setTimeout(function () {
            $.ajax({
                url: '/Profile/FetchPosts',
                type: 'GET',
                data: {
                    skipPost: skip,
                    username: username,
                },
                success: function (data) {
                    if (data.length > 0) {
                        $('#profile-posts-container').append(data);
                        skip++;
                    }
                    $('#profile-posts-container .loading-indicator').remove();
                    isLoading = false;
                },
                error: function () {
                    console.log('Error loading replyposts.');
                    isLoading = false;
                }
            });
        }, delay);
    }
});

$(document).ready(function () {

    if (window.location.pathname !== '/Bookmark') {
        return;
    }

    var skip = 1;
    var isLoading = false;
    loadPosts();

    $('.feed').scroll(function () {
        if ($('.feed').scrollTop() + $('.feed').height() >= $('#bookmark-posts-container').height() + 174) {
            if (!isLoading) {
                loadPosts(1000);
            }
        }
    });

    function loadPosts(delay = 0) {
        isLoading = true;
        const loadingIndicator = '<div class="loading-indicator text-center mt-3"><i class="fa-solid fa-spinner fa-spin fa-2x"></i></div>';

        $('#bookmark-posts-container').append(loadingIndicator);

        setTimeout(function () {
            $.ajax({
                url: '/Bookmark/FetchPosts',
                type: 'GET',
                data: {
                    skipPost: skip,
                },
                success: function (data) {
                    if (data.length > 0) {
                        $('#bookmark-posts-container').append(data);
                        skip++;
                    }
                    $('#bookmark-posts-container .loading-indicator').remove();
                    isLoading = false;
                },
                error: function () {
                    console.log('Error loading replyposts.');
                    isLoading = false;
                }
            });
        }, delay);
    }
});

