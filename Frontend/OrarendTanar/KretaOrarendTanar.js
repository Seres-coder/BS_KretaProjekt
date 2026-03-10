function toggleSidebar(){

	const sidebar = document.getElementById("sidebar");
	const content = document.querySelector(".content");

	sidebar.classList.toggle("closed");
	content.classList.toggle("full");

}