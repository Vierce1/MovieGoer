// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function blockSpace()
{
    
    var e = event || window.event;  // get event object
    var key = e.keyCode || e.which; // get key cross-browser

    if (key == 32) { //if it is not a number ascii code
        //Prevent default action, which is inserting character
        if (e.preventDefault) e.preventDefault(); //normal browsers
        e.returnValue = false; //IE
    }
}

//function changeCheckBox(viewModel, movie)
//{
//    console.error("aslkdfj");
//    viewModel.userWatchedMovies.add

//}