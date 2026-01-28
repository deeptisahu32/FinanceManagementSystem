/* ===== TOGGLE SIDE MENU ===== */
function toggleMenu() {
    document.getElementById("sideMenu").classList.toggle("active");
}


/* ===== SEARCH PRODUCTS ===== */
function searchProducts() {
    const searchValue = document.getElementById("productSearch").value.toLowerCase();
    const products = document.querySelectorAll(".product-card");

    products.forEach(product => {
        const name = product.getAttribute("data-name");
        product.style.display = name.includes(searchValue) ? "block" : "none";
    });
}
