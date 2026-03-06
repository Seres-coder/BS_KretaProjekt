function toggleSidebar(){

	const sidebar = document.getElementById("sidebar");
	const content = document.querySelector(".content");

	sidebar.classList.toggle("closed");
	content.classList.toggle("full");

}

const uzenetek = [
    {
    felado: "Osztályfőnök",
    tema: "Szülői értekezlet",
    datum: "2026.03.05",
    szoveg: "Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz.Kedden 17:00-kor szülői értekezlet lesz."
    },
    
    {
    felado: "Igazgatóság",
    tema: "Tanév rendje",
    datum: "2026.03.04",
    szoveg: "A tavaszi szünet április 2-től kezdődik."
    },
    
    {
    felado: "Matektanár",
    tema: "Dolgozat",
    datum: "2026.03.03",
    szoveg: "A dolgozat jegyei felkerültek a rendszerbe."
    }
    ];
    
    function mutat(i){
    
    let u = uzenetek[i];
    
    document.getElementById("uzenet").innerHTML =
    "<h3>"+u.tema+"</h3>" +
    "<b>"+u.felado+"</b> - "+u.datum+
    "<p>"+u.szoveg+"</p>";
    
    }

    document.querySelectorAll("[data-view]").forEach(gomb => {
        gomb.addEventListener("click", function () {
            let id = this.getAttribute("data-view");
    
            document.querySelectorAll(".panel").forEach(panel => {
                panel.classList.remove("active");
            });
    
            document.getElementById(id).classList.add("active");
        });
    });