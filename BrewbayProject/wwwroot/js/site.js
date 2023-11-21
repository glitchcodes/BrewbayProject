// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const nav = document.querySelector(".custom-nav");
const hamburgerBtn = document.querySelector(".hamburger-btn");
const closeNavBtn = document.querySelector(".close-nav-btn");

const handleNavDisplay = () => {
    nav.classList.toggle("hidden");
};

[hamburgerBtn, closeNavBtn].forEach((docObj) => {
    docObj.addEventListener("click", handleNavDisplay);
});
